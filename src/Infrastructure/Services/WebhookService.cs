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

    public class WebhookService : BaseService, IWebhookService
    {
        private readonly IMediaService _mediaService;
        public WebhookService(IUnitOfWork unitOfWork,
        ILogger<WebhookService> logger, IMediaService mediaService)
         : base(unitOfWork, logger)
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
                    throw new ArgumentException("Context not found");
                }

                if (dto.Payload.Object.Recording_files.Count == default)
                {
                    _logger.LogWarning("No recording files");
                    return;
                }

                if (string.IsNullOrEmpty(dto.Payload.Object.Id.ToString()))
                {
                    _logger.LogWarning("There is no meeting id");
                    return;
                }

                var meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(predicate: p => p.MeetingNumber ==
                            dto.Payload.Object.Id, include: source => source.Include(x => x.Lesson)).ConfigureAwait(false);
                if (meeting == null)
                {
                    _logger.LogWarning("Meeting not found");
                    return;
                }

                if (meeting.Lesson == null && meeting.Lesson?.Type != LessonType.LiveClass)
                {
                    _logger.LogWarning("Lesson is not live class");
                    return;
                }

                var recordingFile = dto.Payload.Object.Recording_files.Where(x => x.File_type.Equals("MP4")).
                                    OrderBy(x => Convert.ToDateTime(x.Recording_start)).ToList();
                var lessons = new List<Lesson>();
                var recordingFileDtos = new List<RecordingFileDto>();
                int order = 1;
                foreach (var file in recordingFile)
                {
                    var mediaFile = await _mediaService.UploadRecordingFileAsync(file.Download_url, dto.Download_token).ConfigureAwait(false);
                    var recording = new RecordingFileDto
                    {
                        Name = $"{meeting.Lesson.Name} Part {order}",
                        Order = order,
                        Key = mediaFile.Key,
                        VideoUrl = mediaFile.Url
                    };
                    recordingFileDtos.Add(recording);
                    order++;
                }

                if (recordingFileDtos.Count > 1)
                {
                    var lessonOrder = meeting.Lesson.Order + 1;
                    var firstRecording = recordingFileDtos.FirstOrDefault(x => x.Order == 1);
                    meeting.Lesson.Type = LessonType.RecordedVideo;
                    meeting.Lesson.VideoUrl = firstRecording.VideoUrl;
                    recordingFileDtos.Remove(firstRecording);
                    var recordings = recordingFileDtos.OrderBy(x => x.Order).ToList();
                    foreach (var fileDto in recordings)
                    {
                        var slug = "hello";
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
                        lessons.Add(lesson);
                        lessonOrder++;
                    }
                    _unitOfWork.GetRepository<Lesson>().Update(meeting.Lesson);
                    await _unitOfWork.GetRepository<Lesson>().InsertAsync(lessons).ConfigureAwait(false);
                }
                else
                {
                    var videoFile = recordingFileDtos.FirstOrDefault();
                    meeting.Lesson.Type = LessonType.RecordedVideo;
                    meeting.Lesson.VideoUrl = videoFile.VideoUrl;
                    _unitOfWork.GetRepository<Lesson>().Update(meeting.Lesson);
                }
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }
    }
}