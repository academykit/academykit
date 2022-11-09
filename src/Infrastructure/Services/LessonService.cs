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
                SectionId = section.Id,
                CreatedBy = currentUserId,
                CreatedOn = currentTimeStamp,
                UpdatedBy = currentUserId,
                UpdatedOn = currentTimeStamp,
                Meeting = new Meeting(),
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
                lesson.Meeting = new Meeting
                {
                    Id = Guid.NewGuid(),
                    StartDate = model.MeetingStartDate,
                    ZoomLicenseId = model.ZoomLicenseId.Value,
                    Duration = model.MeetingDuration,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp
                };
                lesson.MeetingId = lesson.Meeting.Id;
                lesson.Duration = model.MeetingDuration;

                await _unitOfWork.GetRepository<Meeting>().InsertAsync(lesson.Meeting).ConfigureAwait(false);
            }
            await _unitOfWork.GetRepository<Lesson>().InsertAsync(lesson).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return lesson;
        }
    }
}