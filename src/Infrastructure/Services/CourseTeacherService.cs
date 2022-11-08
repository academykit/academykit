namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;

    public class CourseTeacherService : BaseGenericService<CourseTeacher, CourseTeacherSearchCriteria>, ICourseTeacherService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CourseTeacherService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="logger">The logger</param>
        public CourseTeacherService(
            IUnitOfWork unitOfWork,
            ILogger<CourseTeacherService> logger)
            : base(unitOfWork, logger)
        {
        }

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="course teacher"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(CourseTeacher teacher, Guid currentUserId)
        {
            await ValidateAndGetCourse(currentUserId, courseIdentity: teacher.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);
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

            var course = await ValidateAndGetCourse(entity.CreatedBy, courseIdentity: entity.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);
            if (course.CreatedBy == entity.UserId)
            {
                _logger.LogWarning($"course with Id {course.Id} creator User Id {entity.UserId} can't be course teacher.");
                throw new ForbiddenException();
            }
            if (course.CourseTeachers.Any(p => p.UserId == entity.UserId))
            {
                _logger.LogWarning($"User with Id {entity.UserId} is already course teacher of course with Id {course.Id}.");
                throw new ForbiddenException();
            }
            if (course.GroupId.HasValue)
            {
                var canAccess = await ValidateUserCanAccessGroupCourse(course, entity.UserId).ConfigureAwait(false);
                if (!canAccess)
                {
                    _logger.LogWarning($"User with Id {entity.UserId} can't access course with Id {course.Id}.");
                    throw new ForbiddenException("Unauthorized user to added as teacher in group course.");
                }
            }

            var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == entity.UserId).ConfigureAwait(false);
            CommonHelper.CheckFoundEntity(user);
            await Task.FromResult(0);
        }

        protected override async Task SearchPreHookAsync(CourseTeacherSearchCriteria criteria)
        {
            await ValidateAndGetCourse(criteria.CurrentUserId, courseIdentity: criteria.CourseIdentity, validateForModify: false).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<CourseTeacher, bool>> ConstructQueryConditions(Expression<Func<CourseTeacher, bool>> predicate, CourseTeacherSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.CourseIdentity))
            {
                predicate = predicate.And(x => x.Course.Slug == criteria.CourseIdentity || x.Course.Id.ToString() == criteria.CourseIdentity);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.User.Email.Contains(search)
                 || x.User.MobileNumber.Contains(search) || x.User.LastName.Contains(search)
                 || x.User.FirstName.Contains(search));
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
        protected override IIncludableQueryable<CourseTeacher, object> IncludeNavigationProperties(IQueryable<CourseTeacher> query)
        {
            return query.Include(x => x.Course).Include(x => x.User);
        }
    }
}
