using System;
namespace Lingtren.Infrastructure.Services
{
    using System;
    using Hangfire;
    using Hangfire.Server;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Helpers;
    using Microsoft.Extensions.Localization;
    using Lingtren.Infrastructure.Localization;

    public class WebhookService : BaseService, IWebhookService
    {
        private readonly IMediaService _mediaService;
        public WebhookService(IUnitOfWork unitOfWork,
        ILogger<WebhookService> logger, IMediaService mediaService,
        IStringLocalizer<ExceptionLocalizer> localizer)
         : base(unitOfWork, logger,localizer)
        {
            _mediaService = mediaService;
        }

        /// <summary>
        /// Handle to upload zoom recording
        /// </summary>
        /// <param name="dto"> the instance of <see cref="ZoomRecordPayloadDto" />. </param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task UploadZoomRecordingAsync(ZoomRecordPayloadDto dto, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException(_localizer.GetString("ContextNotFound"));
                }

                if (dto.Payload.Object.Recording_files.Count == default)
                {
                    _logger.LogWarning("No recording files.");
                    return;
                }

                if (string.IsNullOrEmpty(dto.Payload.Object.Id.ToString()))
                {
                    _logger.LogWarning("There is no meeting id.");
                    return;
                }

                var meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(predicate: p => p.MeetingNumber ==
                            dto.Payload.Object.Id, include: source => source.Include(x => x.Lesson)).ConfigureAwait(false);
                if (meeting == null)
                {
                    _logger.LogWarning("Meeting not found.");
                    return;
                }

                if (meeting.Lesson == null && meeting.Lesson?.Type != LessonType.LiveClass)
                {
                    _logger.LogWarning("Lesson is not live class.");
                    return;
                }

                var recordingFile = dto.Payload.Object.Recording_files.Where(x => x.File_type.Equals("MP4")).
                                    OrderBy(x => Convert.ToDateTime(x.Recording_start)).ToList();
                var lessons = new List<Lesson>();
                var recordingFileDtos = new List<RecordingFileDto>();
                int order = 1;
                foreach (var file in recordingFile)
                {
                    var videoPath = await _mediaService.UploadRecordingFileAsync(file.Download_url, dto.Download_token,file.File_size).ConfigureAwait(true);
                    var recording = new RecordingFileDto
                    {
                        Name = $"{meeting.Lesson.Name} Part {order}",
                        Order = order,
                        VideoUrl = videoPath
                    };
                    recordingFileDtos.Add(recording);
                    order++;
                }

                if (recordingFileDtos.Count > 1)
                {
                    var sectionLessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.SectionId ==
                                         meeting.Lesson.SectionId).ConfigureAwait(false);
                    var lessonOrderList = sectionLessons.Where(x => x.Order > meeting.Lesson.Order).ToList();
                    if (lessonOrderList.Count != default)
                    {
                        var reoderNo = recordingFileDtos.Count - 1;
                        var reorderLessons = new List<Lesson>();
                        foreach (var lessonorder in lessonOrderList)
                        {
                            lessonorder.Order = lessonorder.Order + reoderNo;
                            reorderLessons.Add(lessonorder);
                        }
                        _unitOfWork.GetRepository<Lesson>().Update(reorderLessons);
                    }

                    var lessonOrder = meeting.Lesson.Order + 1;
                    var firstRecording = recordingFileDtos.FirstOrDefault(x => x.Order == 1);
                    meeting.Lesson.Type = LessonType.RecordedVideo;
                    meeting.Lesson.Name = firstRecording.Name;
                    meeting.Lesson.VideoUrl = firstRecording.VideoUrl;
                    recordingFileDtos.Remove(firstRecording);
                    var recordings = recordingFileDtos.OrderBy(x => x.Order).ToList();
                    foreach (var fileDto in recordings)
                    {
                        var slug = CommonHelper.GetEntityTitleSlug<Lesson>(_unitOfWork, (slug) => q => q.Slug == slug, fileDto.Name);
                        var lesson = new Lesson
                        {
                            Id = Guid.NewGuid(),
                            Name = fileDto.Name,
                            Type = LessonType.RecordedVideo,
                            VideoUrl = fileDto.VideoUrl,
                            Order = lessonOrder,
                            CourseId = meeting.Lesson.CourseId,
                            SectionId = meeting.Lesson.SectionId,
                            Slug = slug,
                            Status = meeting.Lesson.Status,
                            CreatedBy = meeting.Lesson.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };

                        var fileQueue = new VideoQueue
                        {
                            Id = Guid.NewGuid(),
                            LessonId = lesson.Id,
                            VideoUrl = lesson.VideoUrl,
                            CreatedOn = DateTime.UtcNow,
                            Status = VideoStatus.Queue
                        };
                        lessons.Add(lesson);
                        lessonOrder++;
                    }
                    _unitOfWork.GetRepository<Lesson>().Update(meeting.Lesson);
                    await _unitOfWork.GetRepository<Lesson>().InsertAsync(lessons).ConfigureAwait(false);
                }
                else
                {
                    var videos= recordingFileDtos.FirstOrDefault();
                    meeting.Lesson.Type = LessonType.RecordedVideo;
                    meeting.Lesson.VideoUrl = videos.VideoUrl;
                    _unitOfWork.GetRepository<Lesson>().Update(meeting.Lesson);
                }
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                BackgroundJob.Schedule<IZoomLicenseService>(x => x.DeleteZoomMeetingRecordingAsync(dto.Payload.Object.Id,null),TimeSpan.FromMinutes(2880));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to save record of participant join meeting
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomPayLoadDto" /> .</param>
        /// <returns> the task complete </returns>
        public async Task ParticipantJoinMeetingAsync(ZoomPayLoadDto model)
        {
            try
            {
                var meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(predicate: p => p.MeetingNumber.ToString() ==
                          model.Payload.Object.Id, include: source => source.Include(x => x.Lesson)).ConfigureAwait(false);

                if (meeting == default)
                {
                    _logger.LogWarning("Meeting id : {id} not found.", model.Payload.Object.Id);
                    return;
                }

                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p =>
                             p.Id.ToString() == model.Payload.Object.Participant.Customer_Key).ConfigureAwait(false);

                if (user == default)
                {
                    _logger.LogWarning("User with id : {id} not found.", model.Payload.Object.Participant.Customer_Key);
                    return;
                }

                var watchHistory = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(predicate: p => p.CourseId == meeting.Lesson.CourseId &&
                p.LessonId == meeting.Lesson.Id && p.UserId == user.Id).ConfigureAwait(false);
                if(watchHistory == null)
                {
                    var entity = new WatchHistory
                    {
                        Id = Guid.NewGuid(),
                        CourseId = meeting.Lesson.CourseId,
                        LessonId = meeting.Lesson.Id,
                        UserId = user.Id,
                        IsCompleted = true,
                        IsPassed = true,
                        CreatedBy = user.Id,
                        CreatedOn = DateTime.UtcNow,
                    };
                    await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(entity).ConfigureAwait(false);
                }

                var meetingReport = new MeetingReport
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meeting.Id,
                    UserId = user.Id,
                    StartTime = DateTime.Parse(model.Payload.Object.Start_Time),
                    JoinTime = DateTime.Parse(model.Payload.Object.Participant.Join_Time),
                    CreatedOn = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<MeetingReport>().InsertAsync(meetingReport).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to save participant session join record.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to save participant session join record.");
            }
        }

        /// <summary>
        /// Handle to save record of participant leave meeting
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomPayLoadDto" /> .</param>
        /// <returns> the task complete </returns>
        public async Task ParticipantLeaveMeetingAsync(ZoomPayLoadDto model)
        {
            try
            {
                var meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(predicate: x => x.MeetingNumber.ToString() == model.Payload.Object.Id).ConfigureAwait(false);

                if (meeting == default)
                {
                    _logger.LogWarning("Meeting id : {id} not found.", model.Payload.Object.Id);
                    return;
                }
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p =>
                             p.Id.ToString() == model.Payload.Object.Participant.Customer_Key).ConfigureAwait(false);

                if (user == default)
                {
                    _logger.LogWarning("User with id : {id} not found.", model.Payload.Object.Participant.Customer_Key);
                    return;
                }

                var report = await _unitOfWork.GetRepository<MeetingReport>().GetFirstOrDefaultAsync(
                    predicate: p => p.UserId == user.Id && p.MeetingId == meeting.Id
                            && p.StartTime == DateTime.Parse(model.Payload.Object.Start_Time)
                            && p.LeftTime == default).ConfigureAwait(false);
                if (report == default)
                {
                    _logger.LogWarning("Meeting Report not found for user with id : {userId} and meeting with id : {id}.",
                                        user.Id, meeting.Id);
                    return;
                }
                var LeftTime = DateTime.Parse(model.Payload.Object.Participant.Leave_Time);
                report.Duration = LeftTime.Subtract(report.JoinTime);
                report.LeftTime = LeftTime;
                report.UpdatedOn = DateTime.UtcNow;
                _unitOfWork.GetRepository<MeetingReport>().Update(report);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to save participant left record");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to save participant left record");
            }
        }
    }
}