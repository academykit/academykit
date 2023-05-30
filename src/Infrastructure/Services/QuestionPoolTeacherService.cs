namespace Lingtren.Infrastructure.Services
{
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
    using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
    using System.Linq.Expressions;

    public class QuestionPoolTeacherService : BaseGenericService<QuestionPoolTeacher, QuestionPoolTeacherBaseSearchCriteria>, IQuestionPoolTeacherService
    {
        public QuestionPoolTeacherService(IUnitOfWork unitOfWork,
            ILogger<QuestionPoolTeacherService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger,localizer)
        {
        }

        /// <summary>
        /// Handle to assign role
        /// </summary>
        /// <param name="model"> the instance of <see cref="QuestionPoolRoleModel" /> .</param>
        /// <returns> the task complete </returns>
        public async Task AssignRoleAsync(string identity, Guid userId, Guid currentUserId, PoolRole role)
        {
            try
            {
                var questionPool = await _unitOfWork.GetRepository<QuestionPool>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == identity
                                    || p.Slug == identity, include: src => src.Include(y => y.QuestionPoolTeachers)).ConfigureAwait(false);

                if (questionPool == null)
                {
                    _logger.LogWarning("Question pool not found with identity : {identity}.", identity);
                    throw new EntityNotFoundException(_localizer.GetString("QuestionPoolNotFound"));
                }

                var hasTeacher = ValidateQuestionPoolMaintainer(questionPool, currentUserId);
                if (!hasTeacher)
                {
                    throw new ForbiddenException(_localizer.GetString("TeacherRoleInvalidAccess"));
                }

                var user = questionPool?.QuestionPoolTeachers.FirstOrDefault(x => x.UserId == userId);
                if (user == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("QuestionPoolUserNotFound"));
                }

                user.Role = role;
                user.UpdatedBy = currentUserId;
                user.UpdatedOn = DateTime.UtcNow;
                _unitOfWork.GetRepository<QuestionPoolTeacher>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to assign role to question pool.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("AssignRoleQuestionPoolError"));
            }
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="identity">The id or slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<QuestionPoolTeacher, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }

        #region  Private Region

        /// <summary>
        /// Handle to validate question pool maintainer
        /// </summary>
        /// <param name="question"> the instance of <see cref="QuestionPool" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the boolean value </returns>
        private static bool ValidateQuestionPoolMaintainer(QuestionPool question, Guid currentUserId)
        {
            if (question.CreatedBy == currentUserId)
            {
                return true;
            }
            var currentUser = question?.QuestionPoolTeachers.FirstOrDefault(x => x.UserId == currentUserId && x.Role == PoolRole.Creator);
            return currentUser != null;
        }
        #endregion

        #region Protected Region
        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="questionPool teacher"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(QuestionPoolTeacher teacher, Guid currentUserId)
        {
            if (teacher.UserId == currentUserId)
            {
                _logger.LogWarning("User with id : {userId} cannot remove own-self from question pool teacher with pool id : {poolId}.", currentUserId, teacher.QuestionPoolId);
                throw new ForbiddenException(_localizer.GetString("UserCanNotRemoveSameUser"));
            }

            var questionPool = await ValidateAndGetQuestionPool(currentUserId, questionPoolIdentity: teacher.QuestionPoolId.ToString()).ConfigureAwait(false);
            if (questionPool.CreatedBy == teacher.UserId)
            {
                _logger.LogWarning("QuestionPool with Id {id} creator User Id {userId} can't be delete from questionPool teacher.", questionPool.Id, teacher.UserId);
                throw new ForbiddenException(_localizer.GetString("QuestionPoolAuhorCannotRemoved"));
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
        protected override async Task CreatePreHookAsync(QuestionPoolTeacher entity)
        {
            var questionPool = await ValidateAndGetQuestionPool(entity.CreatedBy, questionPoolIdentity: entity.QuestionPoolId.ToString()).ConfigureAwait(false);
            if (questionPool.CreatedBy == entity.UserId)
            {
                _logger.LogWarning("QuestionPool with Id : {id} creator User Id : {userId} can't be questionPool teacher.", questionPool.Id, entity.UserId);
                throw new ForbiddenException(_localizer.GetString("QuestionPoolAuthorCanNotAdded"));
            }
            if (questionPool.QuestionPoolTeachers.Any(p => p.UserId == entity.UserId))
            {
                var poolRole = questionPool.QuestionPoolTeachers.FirstOrDefault(p => p.UserId == entity.UserId)?.Role;
                _logger.LogWarning("User with Id {userId} is already question pool {role} of question pool with Id  : {id}.", entity.UserId, poolRole, questionPool.Id);
                throw new ForbiddenException($"User is already found as question pool {poolRole}.");
            }
            var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == entity.UserId).ConfigureAwait(false);
            CommonHelper.CheckFoundEntity(user);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<QuestionPoolTeacher, bool>> ConstructQueryConditions(Expression<Func<QuestionPoolTeacher, bool>> predicate, QuestionPoolTeacherBaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.QuestionPoolIdentity))
            {
                predicate = predicate.And(x => x.QuestionPool.Slug == criteria.QuestionPoolIdentity || x.QuestionPool.Id.ToString() == criteria.QuestionPoolIdentity);
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
        protected override void SetDefaultSortOption(QuestionPoolTeacherBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(QuestionPoolTeacher.UpdatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<QuestionPoolTeacher, object> IncludeNavigationProperties(IQueryable<QuestionPoolTeacher> query)
        {
            return query.Include(x => x.QuestionPool).Include(x => x.User);
        }
        #endregion Protected Region

    }
}
