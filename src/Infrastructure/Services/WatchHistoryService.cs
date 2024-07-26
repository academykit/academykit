namespace AcademyKit.Infrastructure.Services
{
    using System.Linq.Expressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class WatchHistoryService
        : BaseGenericService<WatchHistory, BaseSearchCriteria>,
            IWatchHistoryService
    {
        public WatchHistoryService(
            IUnitOfWork unitOfWork,
            ILogger<WatchHistoryService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        #region Protected Methods

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<WatchHistory, bool>> ConstructQueryConditions(
            Expression<Func<WatchHistory, bool>> predicate,
            BaseSearchCriteria criteria
        )
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
        protected override IIncludableQueryable<WatchHistory, object> IncludeNavigationProperties(
            IQueryable<WatchHistory> query
        )
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<WatchHistory, bool>> PredicateForIdOrSlug(
            string identity
        )
        {
            return p => p.Id.ToString() == identity;
        }

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="course teacher"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(
            WatchHistory course,
            Guid currentUserId
        )
        {
            await ValidateAndGetCourse(
                    currentUserId,
                    courseIdentity: course.Id.ToString(),
                    validateForModify: true
                )
                .ConfigureAwait(false);
        }
        #endregion Protected Methods

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        public async Task<WatchHistoryResponseModel> CreateAsync(
            WatchHistoryRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id == currentUserId
                        && (p.Role == UserRole.Admin || p.Role == UserRole.SuperAdmin)
                    )
                    .ConfigureAwait(false);
                var course = await ValidateAndGetCourse(
                        currentUserId,
                        model.CourseIdentity,
                        validateForModify: false
                    )
                    .ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning(
                        "Training with identity: {identity} not found for user with :{id}.",
                        model.CourseIdentity,
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.CourseId == course.Id
                        && (
                            p.Id.ToString() == model.LessonIdentity
                            || p.Slug == model.LessonIdentity
                        )
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning(
                        "Lesson with identity: {identity} not found for user with :{id} and training with id : {courseId}.",
                        model.LessonIdentity,
                        currentUserId,
                        course.Id
                    );
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (course.CourseTeachers.Any(x => x.UserId == currentUserId) || user != default)
                {
                    return new WatchHistoryResponseModel();
                }

                var isCompleted = false;
                var isPassed = false;
                var currentTimeStamp = DateTime.UtcNow;
                var response = new WatchHistory();
                if (model.WatchedPercentage == 100)
                {
                    isCompleted = true;
                    if (lesson.Type != LessonType.Assignment && lesson.Type != LessonType.Exam)
                    {
                        isPassed = true;
                    }
                }

                var history = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.CourseId == course.Id
                        && p.LessonId == lesson.Id
                        && p.UserId == currentUserId
                    )
                    .ConfigureAwait(false);

                if (history != null)
                {
                    history.IsCompleted = isCompleted;
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
                        IsCompleted = isCompleted,
                        IsPassed = isPassed,
                        CreatedOn = currentTimeStamp,
                        CreatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                    };
                    await _unitOfWork
                        .GetRepository<WatchHistory>()
                        .InsertAsync(watchHistory)
                        .ConfigureAwait(false);
                    response = watchHistory;
                }

                await ManageStudentCourseComplete(
                        course.Id,
                        lesson.Id,
                        currentUserId,
                        currentTimeStamp
                    )
                    .ConfigureAwait(false);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to pass the student.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorUpdateWatchHistory"));
            }
        }

        /// <summary>
        /// Handle to pass student in requested lesson
        /// </summary>
        /// <param name="userId">the requested user id</param>
        /// <param name="model">the instance of <see cref="WatchHistoryRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task PassAsync(Guid userId, WatchHistoryRequestModel model, Guid currentUserId)
        {
            try
            {
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id == userId
                        && (p.Role == UserRole.Admin || p.Role == UserRole.SuperAdmin)
                    )
                    .ConfigureAwait(false);
                var course = await ValidateAndGetCourse(
                        currentUserId,
                        model.CourseIdentity,
                        validateForModify: true
                    )
                    .ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning(
                        "Training with identity: {identity} not found for user with :{id}.",
                        model.CourseIdentity,
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.CourseId == course.Id
                        && (
                            p.Id.ToString() == model.LessonIdentity
                            || p.Slug == model.LessonIdentity
                        )
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning(
                        "Lesson with identity: {identity} not found for user with :{id} and training with id : {courseId}.",
                        model.LessonIdentity,
                        currentUserId,
                        course.Id
                    );
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (course.CourseTeachers.Any(x => x.UserId != userId) || user == default)
                {
                    var currentTimeStamp = DateTime.UtcNow;
                    var watchHistory = await _unitOfWork
                        .GetRepository<WatchHistory>()
                        .GetFirstOrDefaultAsync(predicate: p =>
                            p.CourseId == course.Id && p.LessonId == lesson.Id && p.UserId == userId
                        )
                        .ConfigureAwait(false);
                    if (watchHistory == null)
                    {
                        var courseEnrollment = await _unitOfWork
                            .GetRepository<CourseEnrollment>()
                            .GetFirstOrDefaultAsync(predicate: p =>
                                p.UserId == userId && p.CourseId == course.Id
                            )
                            .ConfigureAwait(false);
                        var lessonCount = await _unitOfWork
                            .GetRepository<Lesson>()
                            .CountAsync(predicate: p =>
                                p.Course.Id == course.Id && p.IsDeleted != true
                            )
                            .ConfigureAwait(false);

                        watchHistory = new WatchHistory
                        {
                            Id = Guid.NewGuid(),
                            CourseId = course.Id,
                            LessonId = lesson.Id,
                            UserId = userId,
                            IsCompleted = true,
                            IsPassed = false,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp
                        };

                        if (lesson.Type == LessonType.Physical)
                        {
                            var physicalLessonReview = await _unitOfWork
                                .GetRepository<PhysicalLessonReview>()
                                .GetFirstOrDefaultAsync(predicate: p =>
                                    p.UserId == userId && p.LessonId == lesson.Id
                                )
                                .ConfigureAwait(false);
                            physicalLessonReview.IsReviewed = true;
                            physicalLessonReview.HasAttended = true;
                            _unitOfWork
                                .GetRepository<PhysicalLessonReview>()
                                .Update(physicalLessonReview);
                        }

                        courseEnrollment.Percentage =
                            courseEnrollment.Percentage + (100 / lessonCount); //for updating watch percentage when passed by admin
                        courseEnrollment.UpdatedOn = DateTime.UtcNow;
                        courseEnrollment.UpdatedBy = currentUserId;
                        _unitOfWork.GetRepository<CourseEnrollment>().Update(courseEnrollment);

                        await _unitOfWork
                            .GetRepository<WatchHistory>()
                            .InsertAsync(watchHistory)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        watchHistory.IsCompleted = true;
                        watchHistory.IsPassed = true;
                        watchHistory.UpdatedBy = currentUserId;
                        watchHistory.UpdatedOn = DateTime.UtcNow;
                        _unitOfWork.GetRepository<WatchHistory>().Update(watchHistory);
                    }

                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to pass the student.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorUpdateWatchHistory"));
            }
        }

        #region Private Methods

        #endregion Private Methods
    }
}
