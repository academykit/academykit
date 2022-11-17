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

    public class QuestionPoolTeacherService : BaseGenericService<QuestionPoolTeacher, QuestionPoolTeacherBaseSearchCriteria>, IQuestionPoolTeacherService
    {
        public QuestionPoolTeacherService(IUnitOfWork unitOfWork,
            ILogger<QuestionPoolTeacherService> logger) : base(unitOfWork, logger)
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
                    _logger.LogWarning("Question pool not found with identity : {identity}", identity);
                    throw new EntityNotFoundException("Question pool not found");
                }

                var hasTeacher = ValidateQuestionPoolMaintainer(questionPool, currentUserId);
                if (!hasTeacher)
                {
                    throw new ForbiddenException("Invalid access to change question pool teacher's role");
                }

                var user = questionPool?.QuestionPoolTeachers.FirstOrDefault(x => x.UserId == userId);
                if (user == null)
                {
                    throw new EntityNotFoundException("User not found in question pool");
                }

                user.Role = role;
                user.UpdatedBy = currentUserId;
                user.UpdatedOn = DateTime.UtcNow;
                _unitOfWork.GetRepository<QuestionPoolTeacher>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while attempting to assign role to question pool");
                throw ex is ServiceException ? ex : new ServiceException("An error occured while attempting to assign role to question pool");
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
        private bool ValidateQuestionPoolMaintainer(QuestionPool question, Guid currentUserId)
        {
            if (question.CreatedBy == currentUserId)
            {
                return true;
            }
            var currentUser = question?.QuestionPoolTeachers.FirstOrDefault(x => x.UserId == currentUserId && x.Role == PoolRole.Author);
            if (currentUser != null)
            {
                return true;
            }
            return false;
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
            await ValidateAndGetQuestionPool(currentUserId, questionPoolIdentity: teacher.QuestionPoolId.ToString(), validateForModify: true).ConfigureAwait(false);
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

            var questionPool = await ValidateAndGetQuestionPool(entity.CreatedBy, questionPoolIdentity: entity.QuestionPoolId.ToString(), validateForModify: true).ConfigureAwait(false);
            if (questionPool.CreatedBy == entity.UserId)
            {
                _logger.LogWarning($"questionPool with Id {questionPool.Id} creator User Id {entity.UserId} can't be questionPool teacher.");
                throw new ForbiddenException();
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
