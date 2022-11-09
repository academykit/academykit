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

    public class CourseService : BaseGenericService<Course, CourseBaseSearchCriteria>, ICourseService
    {
        public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger) : base(unitOfWork, logger)
        {
        }
        #region Protected Methods
        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Course entity)
        {
            entity.Slug = CommonHelper.GetEntityTitleSlug<Course>(_unitOfWork, (slug) => q => q.Slug == slug, entity.Name);
            await _unitOfWork.GetRepository<CourseTag>().InsertAsync(entity.CourseTags).ConfigureAwait(false);
            await _unitOfWork.GetRepository<CourseTeacher>().InsertAsync(entity.CourseTeachers).ConfigureAwait(false);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        protected override async Task UpdateEntityFieldsAsync(Course existing, Course newEntity)
        {
            if (existing.CourseTags.Count > 0)
            {
                _unitOfWork.GetRepository<CourseTag>().Delete(existing.CourseTags);
            }
            existing.CourseTags = new List<CourseTag>();
            existing.CourseTags.AddRange(newEntity.CourseTags);
            _unitOfWork.DbContext.Entry(existing).CurrentValues.SetValues(newEntity);
            if (existing.CourseTags.Count > 0)
            {
                await _unitOfWork.GetRepository<CourseTag>().InsertAsync(existing.CourseTags).ConfigureAwait(false);
            }
            _unitOfWork.GetRepository<Course>().Update(existing);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Course, bool>> ConstructQueryConditions(Expression<Func<Course, bool>> predicate, CourseBaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
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
        protected override void SetDefaultSortOption(CourseBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Course.CreatedOn);
            criteria.SortType = SortType.Ascending;
        }


        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Course, object> IncludeNavigationProperties(IQueryable<Course> query)
        {
            return query.Include(x => x.User)
                        .Include(x => x.CourseTags)
                        .Include(x => x.Level)
                        .Include(x => x.CourseTeachers);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Course, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="course teacher"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(Course course, Guid currentUserId)
        {
            if (course.Status != CourseStatus.Draft)
            {
                _logger.LogWarning("Course with id : {courseId} cannot be delete having status : {status}", course.Id, course.Status.ToString());
                throw new ForbiddenException("Only draft course can be delete");
            }
            await ValidateAndGetCourse(currentUserId, courseIdentity: course.Id.ToString(), validateForModify: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Check if entity could be accessed by current user
        /// </summary>
        /// <param name="entityToReturn">The entity being returned</param>
        protected override async Task CheckGetPermissionsAsync(Course entityToReturn, Guid? CurrentUserId = null)
        {
            if (!CurrentUserId.HasValue)
            {
                _logger.LogWarning("CurrentUserId is required");
                throw new ForbiddenException("CurrentUserId is required");
            }
            // for creator and course teacher return if exists
            if (entityToReturn.CreatedBy == CurrentUserId || entityToReturn.CourseTeachers.Any(x => x.UserId == CurrentUserId))
            {
                return;
            }
            if (entityToReturn.Status != CourseStatus.Published)
            {
                throw new EntityNotFoundException("The course could not be found");
            }
            if (entityToReturn.GroupId.HasValue)
            {
                var hasAccess = await ValidateUserCanAccessGroupCourse(entityToReturn, CurrentUserId.Value).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException("User not allowed to access this course");
                }
            }
        }
        #endregion Protected Methods

        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="status">the course status</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        public async Task ChangeStatusAsync(string identity, CourseStatus status, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course.Status == status)
            {
                _logger.LogWarning("Course with id : {courseId} cannot be changed to same status by User with id {userId}", course.Id, currentUserId);
                throw new ForbiddenException("Course cannot be changed to same status");
            }
            course.Status = status;
            course.UpdatedBy = currentUserId;
            course.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Course>().Update(course);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}