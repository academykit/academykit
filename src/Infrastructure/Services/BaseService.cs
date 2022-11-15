namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="logger">The logger</param>
        protected BaseService(IUnitOfWork unitOfWork, ILogger logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        protected TResult ExecuteWithResult<TResult>(Func<TResult> delegateFunc)
        {
            try
            {
                return delegateFunc();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        protected async Task<TResult> ExecuteWithResultAsync<TResult>(Func<Task<TResult>> delegateFunc)
        {
            try
            {
                return await delegateFunc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
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
                this._logger.LogError(ex.Message, ex);
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
                this._logger.LogError(ex.Message, ex);
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
            TEntity entity, string argumentName, bool required = false)
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

            TEntity child = _unitOfWork.GetRepository<TEntity>().GetFirstOrDefault(predicate: e => e.Id == entity.Id);

            if (child == null)
            {
                throw new ServiceException(
                    $"Child entity {typeof(TEntity).Name} with Id={entity.Id} was not found.");
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
        protected async Task<Course> ValidateAndGetCourse(Guid currentUserId, string courseIdentity, bool validateForModify = true)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(courseIdentity, nameof(courseIdentity));
            var predicate = PredicateBuilder.New<Course>(true);

            predicate = predicate.And(x => x.Id.ToString() == courseIdentity || x.Slug == courseIdentity);

            var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(
                predicate: predicate,
                include: s => s.Include(x => x.CourseTeachers)
                                .Include(x => x.CourseEnrollments)
                                .Include(x => x.CourseTags)).ConfigureAwait(false);
                                
            CommonHelper.CheckFoundEntity(course);

            if(course.GroupId != default)
            {
                course.Group = new Group();
                course.Group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == course.GroupId,
                    include: src=>src.Include(x=>x.GroupMembers)).ConfigureAwait(false);
            }
           

            // if current user is the creator he can modify/access the course
            if (course.CreatedBy.Equals(currentUserId) || course.CourseTeachers.Any(x => x.UserId == currentUserId))
            {
                return course;
            }

            if (!validateForModify)
            {
                var canAccess = await ValidateUserCanAccessGroupCourse(course, currentUserId).ConfigureAwait(false);
                if (canAccess && course.Status == CourseStatus.Published)
                {
                    return course;
                }
                throw new ForbiddenException("You are not allowed to access this course.");
            }
            throw new ForbiddenException("You are not allowed to modify this course.");
        }

        protected async Task<bool> ValidateUserCanAccessGroupCourse(Course course, Guid currentUserId)
        {
            if (!course.GroupId.HasValue)
            {
                return true;
            }
            var isCourseMember = course.Group.GroupMembers.Any(x => x.UserId == currentUserId);
            return await Task.FromResult(isCourseMember);
        }

        /// <summary>
        /// Validate user and get courses 
        /// </summary>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="courseIdentity">the course id or slug</param>
        /// <param name="validateForModify"></param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        protected async Task<QuestionPool> ValidateAndGetQuestionPool(Guid currentUserId, string questionPoolIdentity, bool validateForModify = true)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(questionPoolIdentity, nameof(questionPoolIdentity));
            var predicate = PredicateBuilder.New<QuestionPool>(true);

            predicate = predicate.And(x => x.Id.ToString() == questionPoolIdentity || x.Slug == questionPoolIdentity);

            var questionPool = await _unitOfWork.GetRepository<QuestionPool>().GetFirstOrDefaultAsync(
                predicate: predicate,
                include: s => s.Include(x => x.QuestionPoolTeachers)).ConfigureAwait(false);

            CommonHelper.CheckFoundEntity(questionPool);

            // if current user is the creator he can modify/access the question pool
            if (questionPool.CreatedBy.Equals(currentUserId) || questionPool.QuestionPoolTeachers.Any(x => x.UserId == currentUserId))
            {
                return questionPool;
            }

            throw new ForbiddenException("You are not allowed to modify this question pool.");
        }
    }
}
