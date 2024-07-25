namespace Lingtren.Infrastructure.Services
{
    using System.Linq.Expressions;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class CourseTeacherService
        : BaseGenericService<CourseTeacher, CourseTeacherSearchCriteria>,
            ICourseTeacherService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CourseTeacherService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="logger">The logger</param>
        public CourseTeacherService(
            IUnitOfWork unitOfWork,
            ILogger<CourseTeacherService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="training trainer"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(
            CourseTeacher teacher,
            Guid currentUserId
        )
        {
            if (teacher.UserId == currentUserId)
            {
                _logger.LogWarning(
                    "User with id : {id} cannot remove own-self from training trainer with course id : {courseId}",
                    currentUserId,
                    teacher.CourseId
                );
                throw new ForbiddenException(_localizer.GetString("SameUserRemoved"));
            }

            var course = await ValidateAndGetCourse(
                    currentUserId,
                    courseIdentity: teacher.CourseId.ToString(),
                    validateForModify: true
                )
                .ConfigureAwait(false);
            if (course.CreatedBy == teacher.UserId)
            {
                _logger.LogWarning(
                    "Training with id {id} creator User Id {userId} can't be delete from training trainer.",
                    course.Id,
                    teacher.UserId
                );
                throw new ForbiddenException(_localizer.GetString("TrainingAuthorRemoved"));
            }
        }

        /// <summary>
        /// Updates the child entities by loading them from the database and validating.
        /// </summary>
        ///
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        ///
        /// <param name="entity">The current entity.</param>
        protected override async Task CreatePreHookAsync(CourseTeacher entity)
        {
            var course = await ValidateAndGetCourse(
                    entity.CreatedBy,
                    courseIdentity: entity.CourseId.ToString(),
                    validateForModify: true
                )
                .ConfigureAwait(false);
            if (course.CreatedBy == entity.UserId)
            {
                _logger.LogWarning(
                    "Training with id {courseId} creator User Id {userId} can't be training trainer.",
                    course.Id,
                    entity.UserId
                );
                throw new ForbiddenException(_localizer.GetString("TrainingAuthorAdded"));
            }

            if (course.CourseEnrollments.Any(x => x.UserId == entity.UserId))
            {
                _logger.LogWarning(
                    "Training with id {courseId} User with UserID{userId} cant be training trainer.",
                    course.Id,
                    entity.UserId
                );
                throw new ForbiddenException(_localizer.GetString("EnrolledUserCan'tBeTrainer"));
            }

            var hasAccess = await IsSuperAdminOrAdminOrTrainer(entity.CreatedBy)
                .ConfigureAwait(false);
            if (!hasAccess)
            {
                _logger.LogWarning(
                    "User having Id: {userId} with trainee role is not allowed to added as training trainer of training with id {courseId}.",
                    entity.UserId,
                    course.Id
                );
                throw new ForbiddenException(
                    _localizer.GetString("TraineeAsTrainingTrainerNotAdded")
                );
            }

            if (course.CourseTeachers.Any(p => p.UserId == entity.UserId))
            {
                _logger.LogWarning(
                    "User with Id {userId} is already training trainer of training with id {courseId}.",
                    entity.UserId,
                    course.Id
                );
                throw new ForbiddenException(_localizer.GetString("UserFoundasTrainingTrainer"));
            }

            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(entity.UserId)
                .ConfigureAwait(false);
            if (!isSuperAdminOrAdmin)
            {
                if (course.GroupId.HasValue)
                {
                    var canAccess = await ValidateUserCanAccessGroupCourse(course, entity.UserId)
                        .ConfigureAwait(false);
                    if (!canAccess)
                    {
                        var groupMember = new GroupMember
                        {
                            Id = Guid.NewGuid(),
                            GroupId = course.GroupId.Value,
                            IsActive = true,
                            UserId = entity.UserId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };
                        await _unitOfWork
                            .GetRepository<GroupMember>()
                            .InsertAsync(groupMember)
                            .ConfigureAwait(false);
                    }
                }
            }

            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == entity.UserId)
                .ConfigureAwait(false);
            CommonHelper.CheckFoundEntity(user);
            await Task.FromResult(0);
        }

        protected override async Task SearchPreHookAsync(CourseTeacherSearchCriteria criteria)
        {
            await ValidateAndGetCourse(
                    criteria.CurrentUserId,
                    courseIdentity: criteria.CourseIdentity,
                    validateForModify: false
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<CourseTeacher, bool>> ConstructQueryConditions(
            Expression<Func<CourseTeacher, bool>> predicate,
            CourseTeacherSearchCriteria criteria
        )
        {
            if (!string.IsNullOrWhiteSpace(criteria.CourseIdentity))
            {
                predicate = predicate.And(x =>
                    x.Course.Slug == criteria.CourseIdentity
                    || x.Course.Id.ToString() == criteria.CourseIdentity
                );
            }

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x =>
                    x.User.Email.Contains(search)
                    || x.User.MobileNumber.Contains(search)
                    || x.User.LastName.Contains(search)
                    || x.User.FirstName.Contains(search)
                );
            }

            return predicate;
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(CourseTeacherSearchCriteria criteria)
        {
            criteria.SortBy = nameof(CourseTeacher.UpdatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<CourseTeacher, object> IncludeNavigationProperties(
            IQueryable<CourseTeacher> query
        )
        {
            return query.Include(x => x.Course).Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="identity">The id or slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<CourseTeacher, bool>> PredicateForIdOrSlug(
            string identity
        )
        {
            return p => p.Id.ToString() == identity;
        }
    }
}
