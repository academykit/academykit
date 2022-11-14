namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Microsoft.Extensions.Logging;

    public class LessonService : BaseGenericService<Lesson, BaseSearchCriteria>, ILessonService
    {
        public LessonService(
            IUnitOfWork unitOfWork,
            ILogger<LessonService> logger) : base(unitOfWork, logger)
        {
        }

        /// <summary>
        /// Handle to create lesson
        /// </summary>
        /// <param name="courseIdentity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></see></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Lesson> AddAsync(string courseIdentity, LessonRequestModel model, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, courseIdentity, validateForModify: true).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Course with identity: {identity} not found for user with :{id}", courseIdentity, currentUserId);
                throw new EntityNotFoundException("Course not found");
            }
            var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                predicate: p => p.Id.ToString() == model.SectionIdentity || p.Slug == model.SectionIdentity).ConfigureAwait(false);

            var currentTimeStamp = DateTime.UtcNow;

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                Name = model.Name,
                Description = model.Description,
                ThumbnailUrl = model.ThumbnailUrl,
                Type = model.Type,
                IsPreview = model.IsPreview,
                IsMandatory = model.IsMandatory,
                SectionId = section.Id,
                CreatedBy = currentUserId,
                CreatedOn = currentTimeStamp,
                UpdatedBy = currentUserId,
                UpdatedOn = currentTimeStamp,
            };
            lesson.Slug = CommonHelper.GetEntityTitleSlug<Course>(_unitOfWork, (slug) => q => q.Slug == slug, lesson.Name);

            if (lesson.Type == LessonType.Document)
            {
                lesson.DocumentUrl = model.DocumentUrl;
            }
            if (lesson.Type == LessonType.Video)
            {
                lesson.VideoUrl = model.VideoUrl;
            }
            if (lesson.Type == LessonType.LiveClass)
            {
                await CreateMeetingAsync(model, lesson).ConfigureAwait(false);
            }
            if (lesson.Type == LessonType.Exam)
            {
                await CreateQuestionSetAsync(model, lesson).ConfigureAwait(false);
            }
            await _unitOfWork.GetRepository<Lesson>().InsertAsync(lesson).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return lesson;
        }

        /// <summary>
        /// Handle to  create question set
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="lesson"></param>
        /// <returns></returns>

        private async Task CreateQuestionSetAsync(LessonRequestModel model, Lesson lesson)
        {
            lesson.QuestionSet = new QuestionSet();
            lesson.QuestionSet = new QuestionSet
            {
                Id = Guid.NewGuid(),
                Slug = string.Concat(lesson.Slug, "-", lesson.Id.ToString().AsSpan(0, 5)),
                Name = lesson.Name,
                ThumbnailUrl = lesson.ThumbnailUrl,
                Description = lesson.Description,
                NegativeMarking = lesson.QuestionSet.NegativeMarking,
                QuestionMarking = lesson.QuestionSet.QuestionMarking,
                PassingWeightage = lesson.QuestionSet.PassingWeightage,
                AllowedRetake = lesson.QuestionSet.AllowedRetake,
                StartTime = lesson.QuestionSet.StartTime,
                EndTime = lesson.QuestionSet.EndTime,
                Duration = lesson.QuestionSet.Duration,
                CreatedBy = lesson.CreatedBy,
                CreatedOn = lesson.CreatedOn,
                UpdatedBy = lesson.UpdatedBy,
                UpdatedOn = lesson.UpdatedOn
            };
            lesson.Duration = model.QuestionSet.Duration;
            lesson.QuestionSetId = lesson.QuestionSet.Id;

            await _unitOfWork.GetRepository<QuestionSet>().InsertAsync(lesson.QuestionSet).ConfigureAwait(false);
        }

        private async Task CreateMeetingAsync(LessonRequestModel model, Lesson lesson)
        {
            lesson.Meeting = new Meeting();
            lesson.Meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                StartDate = model.Meeting.MeetingStartDate,
                ZoomLicenseId = model.Meeting.ZoomLicenseId.Value,
                Duration = model.Meeting.MeetingDuration,
                CreatedBy = lesson.CreatedBy,
                CreatedOn = lesson.CreatedOn,
                UpdatedBy = lesson.UpdatedBy,
                UpdatedOn = lesson.UpdatedOn
            };
            lesson.MeetingId = lesson.Meeting.Id;
            lesson.Duration = model.Meeting.MeetingDuration;

            await _unitOfWork.GetRepository<Meeting>().InsertAsync(lesson.Meeting).ConfigureAwait(false);
        }
    }
}