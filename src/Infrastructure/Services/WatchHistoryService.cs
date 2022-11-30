namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;

    public class WatchHistoryService : BaseGenericService<WatchHistory, BaseSearchCriteria>, IWatchHistoryService
    {
        public WatchHistoryService(
            IUnitOfWork unitOfWork,
            ILogger<WatchHistoryService> logger) : base(unitOfWork, logger)
        {
        }

        #region Protected Methods

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<WatchHistory, bool>> ConstructQueryConditions(Expression<Func<WatchHistory, bool>> predicate, BaseSearchCriteria criteria)
        {
            return predicate;
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(BaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Course.CreatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<WatchHistory, object> IncludeNavigationProperties(IQueryable<WatchHistory> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<WatchHistory, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="course teacher"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(WatchHistory course, Guid currentUserId)
        {
            //if (course.Status != CourseStatus.Draft)
            //{
            //    _logger.LogWarning("Course with id : {courseId} cannot be delete having status : {status}", course.Id, course.Status.ToString());
            //    throw new ForbiddenException("Only draft course can be delete");
            //}
            await ValidateAndGetCourse(currentUserId, courseIdentity: course.Id.ToString(), validateForModify: true).ConfigureAwait(false);
        }
        #endregion Protected Methods

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        public async Task<WatchHistoryResponseModel> CreateAsync(WatchHistoryRequestModel model, Guid currentUserId)
        {

            var course = await ValidateAndGetCourse(currentUserId, model.CourseIdentity, validateForModify: false).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Course with identity: {identity} not found for user with :{id}", model.CourseIdentity, currentUserId);
                throw new EntityNotFoundException("Course not found");
            }
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id && (p.Id.ToString() == model.LessonIdentity || p.Slug == model.LessonIdentity)).ConfigureAwait(false);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with identity: {identity} not found for user with :{id} and course with id : {courseId}", model.LessonIdentity, currentUserId, course.Id);
                throw new EntityNotFoundException("Lesson not found");
            }

            var isComplete = false;
            var currentTimeStamp = DateTime.UtcNow;
            var response = new WatchHistory();
            if (model.WatchedPercentage == 100)
            {
                isComplete = true;
            }

            var history = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id && p.LessonId == lesson.Id && p.UserId == currentUserId).ConfigureAwait(false);

            if (history != null)
            {
                history.IsCompleted = isComplete;
                history.UpdatedOn = currentTimeStamp;
                history.UpdatedBy = currentUserId;
                _unitOfWork.GetRepository<WatchHistory>().Update(history);
                response = history;
            }
            else
            {
                var watchHistory = new WatchHistory
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    LessonId = lesson.Id,
                    UserId = currentUserId,
                    IsCompleted = isComplete,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                };
                await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(watchHistory).ConfigureAwait(false);
                response = watchHistory;
            }

            var percentage = await this.GetCourseCompletedPercentage(course.Id, currentUserId).ConfigureAwait(false);

            var courseEnrollment = await _unitOfWork.GetRepository<CourseEnrollment>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id && p.UserId == currentUserId
                                && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)
                ).ConfigureAwait(false);

            if (courseEnrollment != null)
            {
                courseEnrollment.Percentage = percentage;
                courseEnrollment.CurrentLessonId = lesson.Id;
                courseEnrollment.UpdatedBy = currentUserId;
                courseEnrollment.UpdatedOn = currentTimeStamp;
                if (percentage == 100)
                {
                    courseEnrollment.EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Completed;
                }
                _unitOfWork.GetRepository<CourseEnrollment>().Update(courseEnrollment);
            }
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return new WatchHistoryResponseModel
            {
                Id = response.Id,
                CourseSlug = course.Slug,
                LessonSlug = lesson.Slug,
                IsCompleted = response.IsCompleted,
                WatchedDate = Convert.ToDateTime(response.UpdatedOn)
            };
        }

        /// <summary>
        ///  Handle to get course completed percentage
        /// </summary>
        /// <param name="courseId"> the course id </param>
        /// <param name="currentUserId"> the lesson id</param>
        /// <returns> the percentage </returns>
        private async Task<int> GetCourseCompletedPercentage(Guid courseId, Guid currentUserId)
        {
            try
            {
                var totalLessonCount = await _unitOfWork.GetRepository<Lesson>().CountAsync(
                    predicate: p => p.CourseId == courseId && !p.IsDeleted && p.Status == CourseStatus.Published).ConfigureAwait(false);
                var completedLessonCount = await _unitOfWork.GetRepository<WatchHistory>().CountAsync(
                    predicate: p => p.CourseId == courseId && p.UserId == currentUserId && p.IsCompleted).ConfigureAwait(false);
                var percentage = (Convert.ToDouble(completedLessonCount) / Convert.ToDouble(totalLessonCount)) * 100;
                var result = Convert.ToInt32(percentage);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while trying to calculate course completed percentage.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to calculate course completed percentage.");
            }
        }

    }
}
