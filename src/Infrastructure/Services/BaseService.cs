namespace Lingtren.Infrastructure.Services
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public abstract class BaseService
    {
        /// <summary>
        /// The unit of work.
        /// </summary>
        protected readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger _logger;

        protected readonly IStringLocalizer<ExceptionLocalizer> _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="logger">The logger</param>
        protected BaseService(
            IUnitOfWork unitOfWork,
            ILogger logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        protected TResult ExecuteWithResult<TResult>(Func<TResult> delegateFunc)
        {
            try
            {
                return delegateFunc();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        protected async Task<TResult> ExecuteWithResultAsync<TResult>(
            Func<Task<TResult>> delegateFunc
        )
        {
            try
            {
                return await delegateFunc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        protected async Task Execute(Func<Task> delegateFunc)
        {
            try
            {
                await delegateFunc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        protected async Task ExecuteAsync(Func<Task> delegateFunc)
        {
            try
            {
                await delegateFunc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Check that entity is not <c>null</c> and tries to retrieve its updated value from the database context.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="context">The database context.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="argumentName">The name of the argument being validated.</param>
        /// <param name="required">Determines whether entity should not be null.</param>
        /// <returns>The updated entity from the database context.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="entity"/> is null and <paramref name="required"/> is True.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If entity with given Id or Name (for lookup entity with Id=0) was not found.
        /// </exception>
        /// <remarks>All other exceptions will be propagated to caller method.</remarks>
        protected TEntity ResolveChildEntity<TEntity>(
            TEntity entity,
            string argumentName,
            bool required = false
        )
            where TEntity : IdentifiableEntity
        {
            if (entity == null)
            {
                if (!required)
                {
                    return null;
                }

                argumentName = typeof(TEntity).Name + "." + argumentName;
                throw new ArgumentException($"{argumentName} cannot be null.", argumentName);
            }

            var child = _unitOfWork
                .GetRepository<TEntity>()
                .GetFirstOrDefault(predicate: e => e.Id == entity.Id);

            if (child == null)
            {
                throw new ServiceException(
                    $"Child entity {typeof(TEntity).Name} with Id={entity.Id} was not found."
                );
            }

            return child;
        }

        /// <summary>
        /// Validate user and get courses
        /// </summary>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="courseIdentity">the course id or slug</param>
        /// <param name="validateForModify"></param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        protected async Task<Course> ValidateAndGetCourse(
            Guid currentUserId,
            string courseIdentity,
            bool validateForModify = true
        )
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(courseIdentity, nameof(courseIdentity));
            var predicate = PredicateBuilder.New<Course>(true);

            predicate = predicate.And(x =>
                x.Id.ToString() == courseIdentity || x.Slug == courseIdentity
            );

            var course = await _unitOfWork
                .GetRepository<Course>()
                .GetFirstOrDefaultAsync(
                    predicate: predicate,
                    include: s =>
                        s.Include(x => x.CourseTeachers)
                            .Include(x => x.User)
                            .Include(x => x.CourseEnrollments)
                            .Include(x => x.CourseTags)
                            .Include(x => x.TrainingEligibilities)
                )
                .ConfigureAwait(false);

            CommonHelper.CheckFoundEntity(course);

            if (course.GroupId != default)
            {
                course.Group = new Group();
                course.Group = await _unitOfWork
                    .GetRepository<Group>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == course.GroupId,
                        include: src => src.Include(x => x.GroupMembers.Where(x => x.IsActive))
                    )
                    .ConfigureAwait(false);
            }

            var isSuperAdminOrAdminAccess = await IsSuperAdminOrAdmin(currentUserId)
                .ConfigureAwait(false);

            if (
                course.CreatedBy.Equals(currentUserId)
                || course.CourseTeachers.Any(x => x.UserId == currentUserId)
                || isSuperAdminOrAdminAccess
            )
            {
                return course;
            }

            if (!validateForModify)
            {
                var canAccess = await ValidateUserCanAccessGroupCourse(course, currentUserId)
                    .ConfigureAwait(false);
                if (
                    canAccess
                    && (
                        course.IsUpdate
                        || course.Status == CourseStatus.Published
                        || course.Status == CourseStatus.Completed
                    )
                )
                {
                    return course;
                }

                throw new ForbiddenException(_localizer.GetString("Trainingaccessnotallowed"));
            }

            throw new ForbiddenException(_localizer.GetString("TrainingModifynotallowed"));
        }

        protected async Task<bool> ValidateUserCanAccessGroupCourse(
            Course course,
            Guid currentUserId
        )
        {
            if (!course.GroupId.HasValue)
            {
                return true;
            }

            var isCourseMember = await _unitOfWork
                .GetRepository<Group>()
                .ExistsAsync(predicate: p =>
                    p.Courses.Any(x => x.Id == course.Id)
                    && p.GroupMembers.Any(x =>
                        x.GroupId == course.GroupId && x.UserId == currentUserId && x.IsActive
                    )
                )
                .ConfigureAwait(false);

            return await Task.FromResult(isCourseMember);
        }

        protected async Task<bool> ValidateUserCanAccessGroup(Guid groupId, Guid currentUserId)
        {
            var isGroupMember = await _unitOfWork
                .GetRepository<GroupMember>()
                .ExistsAsync(predicate: p =>
                    p.GroupId == groupId && p.UserId == currentUserId && p.IsActive
                )
                .ConfigureAwait(false);
            return await Task.FromResult(isGroupMember);
        }

        /// <summary>
        /// Validate user and get courses
        /// </summary>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="questionPoolIdentity">the question pool id or slug</param>
        /// <param name="validateForModify"></param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        protected async Task<QuestionPool> ValidateAndGetQuestionPool(
            Guid currentUserId,
            string questionPoolIdentity
        )
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                questionPoolIdentity,
                nameof(questionPoolIdentity)
            );
            var predicate = PredicateBuilder.New<QuestionPool>(true);

            predicate = predicate.And(x =>
                x.Id.ToString() == questionPoolIdentity || x.Slug == questionPoolIdentity
            );

            var questionPool = await _unitOfWork
                .GetRepository<QuestionPool>()
                .GetFirstOrDefaultAsync(
                    predicate: predicate,
                    include: s => s.Include(x => x.QuestionPoolTeachers)
                )
                .ConfigureAwait(false);

            CommonHelper.CheckFoundEntity(questionPool);

            // if current user is the creator he can modify/access the question pool
            if (
                questionPool.CreatedBy.Equals(currentUserId)
                || questionPool.QuestionPoolTeachers.Any(x => x.UserId == currentUserId)
            )
            {
                return questionPool;
            }

            throw new ForbiddenException(_localizer.GetString("QuestionpoolModifynotallowed"));
        }

        protected async Task<IList<Guid>> GetUserGroupIds(Guid userId)
        {
            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == userId,
                    include: src => src.Include(x => x.GroupMembers.Where(x => x.IsActive))
                )
                .ConfigureAwait(false);
            return user?.GroupMembers?.Select(x => x.GroupId).ToList();
        }

        protected async Task<bool> IsSuperAdmin(Guid currentUserId)
        {
            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id == currentUserId
                    && p.Status == UserStatus.Active
                    && p.Role == UserRole.SuperAdmin
                )
                .ConfigureAwait(false);

            return user != null;
        }

        protected async Task<bool> IsSuperAdminOrAdmin(Guid currentUserId)
        {
            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id == currentUserId
                    && p.Status == UserStatus.Active
                    && (p.Role == UserRole.SuperAdmin || p.Role == UserRole.Admin)
                )
                .ConfigureAwait(false);
            return user != null;
        }

        protected async Task<bool> IsSuperAdminOrAdminOrTrainer(Guid currentUserId)
        {
            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id == currentUserId
                    && p.Status == UserStatus.Active
                    && (
                        p.Role == UserRole.SuperAdmin
                        || p.Role == UserRole.Admin
                        || p.Role == UserRole.Trainer
                    )
                )
                .ConfigureAwait(false);
            return user != null;
        }

        protected async Task<bool> IsTrainer(Guid currentUserId)
        {
            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id == currentUserId
                    && p.Status == UserStatus.Active
                    && p.Role == UserRole.Trainer
                )
                .ConfigureAwait(false);
            return user != null;
        }

        /// <summary>
        /// to get valid user for course and questionpool authority
        /// </summary>
        /// <param name="currentuserId">Current userId</param>
        /// <param name="identity">Training Identity</param>
        /// <param name="trainingType">the instance of<see cref="TrainingTypeEnum"/></param>
        /// <returns>bool</returns>
        protected async Task<bool> IsSuperAdminOrAdminOrTrainerOfTraining(
            Guid currentuserId,
            string identity,
            TrainingTypeEnum trainingType
        )
        {
            var isValidUser = false;
            var IsAdimOrSuperAdmin = await IsSuperAdminOrAdmin(currentuserId);
            switch (trainingType)
            {
                case TrainingTypeEnum.Course:
                    var course = await _unitOfWork
                        .GetRepository<Course>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.Id.ToString() == identity || p.Slug == identity,
                            include: src => src.Include(x => x.CourseTeachers)
                        )
                        .ConfigureAwait(false);
                    if (course == default)
                    {
                        throw new EntityNotFoundException(_localizer.GetString("CourseNotFound"));
                    }

                    isValidUser =
                        course.CourseTeachers.Any(x => x.UserId == currentuserId)
                        || course.CreatedBy == currentuserId;
                    break;
                case TrainingTypeEnum.QuestionPool:
                    var questionpool = await _unitOfWork
                        .GetRepository<QuestionPool>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.Id.ToString() == identity || p.Slug == identity,
                            include: src => src.Include(x => x.QuestionPoolTeachers)
                        )
                        .ConfigureAwait(false);
                    if (questionpool == default)
                    {
                        throw new EntityNotFoundException(
                            _localizer.GetString("QuestionPoolNotFound")
                        );
                    }

                    isValidUser =
                        questionpool.QuestionPoolTeachers.Any(x => x.UserId == currentuserId)
                        || questionpool.CreatedBy == currentuserId;
                    break;
            }

            return isValidUser || IsAdimOrSuperAdmin;
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
                var totalLessonCount = await _unitOfWork
                    .GetRepository<Lesson>()
                    .CountAsync(predicate: p =>
                        p.CourseId == courseId && !p.IsDeleted && p.Status == CourseStatus.Published
                    )
                    .ConfigureAwait(false);
                var completedLessonCount = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .CountAsync(predicate: p =>
                        p.CourseId == courseId && p.UserId == currentUserId && p.IsCompleted
                    )
                    .ConfigureAwait(false);
                var percentage =
                    totalLessonCount == 0
                        ? 0
                        : (
                            Convert.ToDouble(completedLessonCount + 1)
                            / Convert.ToDouble(totalLessonCount)
                        ) * 100;
                var result = Convert.ToInt32(percentage);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while trying to calculate training completed percentage."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("TrainingCompletePercentage"));
            }
        }

        /// <summary>
        /// Handle to manage student course complete state
        /// </summary>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="courseId">the course id <see </param>
        /// <param name="lessonId">the lesson id</param>
        /// <param name="currentTimeStamp">the current time stamp</param>
        /// <returns>the task complete</returns>
        protected async Task ManageStudentCourseComplete(
            Guid courseId,
            Guid lessonId,
            Guid currentUserId,
            DateTime currentTimeStamp
        )
        {
            var percentage = await GetCourseCompletedPercentage(courseId, currentUserId)
                .ConfigureAwait(false);

            var courseEnrollment = await _unitOfWork
                .GetRepository<CourseEnrollment>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.CourseId == courseId
                    && p.UserId == currentUserId
                    && (
                        p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled
                        || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed
                    )
                )
                .ConfigureAwait(false);

            if (courseEnrollment != null)
            {
                courseEnrollment.Percentage = percentage;
                courseEnrollment.CurrentLessonId = lessonId;
                courseEnrollment.UpdatedBy = currentUserId;
                courseEnrollment.UpdatedOn = currentTimeStamp;
                if (percentage == 100)
                {
                    courseEnrollment.EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Completed;
                }

                _unitOfWork.GetRepository<CourseEnrollment>().Update(courseEnrollment);
            }
        }
    }
}
