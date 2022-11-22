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
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Course, bool>> ConstructQueryConditions(Expression<Func<Course, bool>> predicate, CourseBaseSearchCriteria criteria)
        {
            if (criteria.Status.HasValue)
            {
                predicate = predicate.And(p => p.Status.Equals(criteria.Status.Value));
            }
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }

            if (criteria.CurrentUserId == default)
            {
                return predicate.And(x => x.Status == CourseStatus.Published);
            }

            if (criteria.EnrollmentStatus?.Count > 0)
            {
                var enrollmentStatusPredicate = PredicateBuilder.New<Course>();
                foreach (var enrollmentStatus in criteria.EnrollmentStatus)
                {
                    switch (enrollmentStatus)
                    {
                        case CourseEnrollmentStatus.Enrolled:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => p.CourseEnrollments.Any(e => e.UserId == criteria.CurrentUserId));
                            break;
                        case CourseEnrollmentStatus.NotEnrolled:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => !p.CourseEnrollments.Any(e => e.UserId == criteria.CurrentUserId));
                            break;
                        case CourseEnrollmentStatus.Host:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => p.CreatedBy == criteria.CurrentUserId);
                            break;
                        case CourseEnrollmentStatus.Moderator:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => p.CourseTeachers.Any(e => e.UserId == criteria.CurrentUserId));
                            break;
                        default:
                            break;
                    }
                }
                predicate = predicate.And(enrollmentStatusPredicate);
            }

            return predicate.And(x => x.CreatedBy == criteria.CurrentUserId
            || (x.CreatedBy != criteria.CurrentUserId && x.Status.Equals(CourseStatus.Published)));
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
            criteria.SortType = SortType.Descending;
        }


        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Course, object> IncludeNavigationProperties(IQueryable<Course> query)
        {
            return query.Include(x => x.User)
                        .Include(x => x.CourseTags).ThenInclude(x => x.Tag)
                        .Include(x => x.Level)
                        .Include(x => x.Group)
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

        /// <summary>
        /// Handel to populate live session retrieved entity
        /// </summary>
        /// <param name="entity">the instance of <see cref="LiveSession"/></param>
        /// <returns></returns>
        protected override async Task PopulateRetrievedEntity(Course entity)
        {
            var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(predicate: p => p.CourseId == entity.Id,
                include: src => src.Include(x => x.Lessons).Include(x => x.User)).ConfigureAwait(false);
            entity.Sections = sections;
        }
        #endregion Protected Methods


        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CourseRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task<Course> UpdateAsync(string identity, CourseRequestModel model, Guid currentUserId)
        {
            var existing = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.Language = model.Language;
            existing.GroupId = model.GroupId;
            existing.LevelId = model.LevelId;
            existing.Duration = model.Duration;
            existing.Description = model.Description;
            existing.ThumbnailUrl = model.ThumbnailUrl;
            existing.UpdatedBy = currentUserId;
            existing.UpdatedOn = currentTimeStamp;

            var newCourseTags = new List<CourseTag>();

            foreach (var tagId in model.TagIds)
            {
                newCourseTags.Add(new CourseTag
                {
                    Id = Guid.NewGuid(),
                    TagId = tagId,
                    CourseId = existing.Id,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                });
            }

            if (existing.CourseTags.Count > 0)
            {
                _unitOfWork.GetRepository<CourseTag>().Delete(existing.CourseTags);
            }
            if (newCourseTags.Count > 0)
            {
                await _unitOfWork.GetRepository<CourseTag>().InsertAsync(newCourseTags).ConfigureAwait(false);
            }
            _unitOfWork.GetRepository<Course>().Update(existing);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return await GetByIdOrSlugAsync(identity,currentUserId).ConfigureAwait(false);
        }

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

        /// <summary>
        /// Course Enrollment
        /// </summary>
        /// <param name="identity"> course id or slug</param>
        /// <param name="userId"> the user id</param>

        public async Task EnrollmentAsync(string identity, Guid userId)
        {
            await ExecuteAsync(async () =>
            {
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                    predicate: x => x.Id == userId,
                    include: p => p.Include(x => x.CourseEnrollments)).ConfigureAwait(false);
                CommonHelper.CheckFoundEntity(user);

                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(
                    predicate: x => x.Id.ToString() == identity && x.Slug == identity,
                    include: src => src.Include(x => x.User).Include(x => x.CourseEnrollments)
                    ).ConfigureAwait(false);
                CommonHelper.CheckFoundEntity(course);

                if (course.CourseEnrollments.Any(p => p.UserId == userId
                            && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)))
                {
                    _logger.LogWarning("User with userId: {userId} is already enrolled in the course with id: {courseId}", userId, course.Id);
                    throw new ArgumentException("You are already enrolled in this course");
                }

                var currentTimeStamp = DateTime.UtcNow;
                CourseEnrollment courseEnrollment = new CourseEnrollment()
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    EnrollmentDate = currentTimeStamp,
                    CreatedBy = userId,
                    CreatedOn = currentTimeStamp,
                    UserId = userId,
                    UpdatedBy = userId,
                    UpdatedOn = currentTimeStamp,
                    EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Enrolled,
                };

                await _unitOfWork.GetRepository<CourseEnrollment>().InsertAsync(courseEnrollment).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Handle to delete course 
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the task complete</returns>
        public async Task DeleteCourseAsync(string identity, Guid currentUserId)
        {
            await ExecuteAsync(async () =>
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Course with identity : {0} not found for user with id : {1}", identity, currentUserId);
                    throw new EntityNotFoundException("Course not found");
                }
                if (course.Status != CourseStatus.Draft)
                {
                    _logger.LogWarning("Course with identity : {0} is in {1} status. So, it cannot be removed", identity, course.Status);
                    throw new EntityNotFoundException("Course with draft status is only allowed to removed.");
                }
                if (course.CourseEnrollments.Count > 0)
                {
                    _logger.LogWarning("Course with identity : {0} contains enrollments", identity);
                    throw new EntityNotFoundException("Course contains member enrollments. So, it cannot be removed");
                }

                var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.CourseId == course.Id).ConfigureAwait(false);

                _unitOfWork.GetRepository<Section>().Delete(sections);
                _unitOfWork.GetRepository<Lesson>().Delete(lessons);
                _unitOfWork.GetRepository<CourseTag>().Delete(course.CourseTags);
                _unitOfWork.GetRepository<CourseTeacher>().Delete(course.CourseTeachers);
                _unitOfWork.GetRepository<Course>().Delete(course);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            });
        }
    }
}