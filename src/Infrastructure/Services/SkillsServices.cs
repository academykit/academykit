using System.Data;
using System.Linq.Expressions;
using System.Text;
using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Domain.Entities;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Localization;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Lingtren.Infrastructure.Services
{
    public class SkillsServices
        : BaseGenericService<Skills, SkillsBaseSearchCriteria>,
            ISkillService
    {
        public SkillsServices(
            IUnitOfWork unitOfWork,
            ILogger<SkillsServices> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        #region Protected Methods
        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Skills entity)
        {
            await CheckDuplicateSkillsNameAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        protected override async Task UpdateEntityFieldsAsync(Skills existing, Skills newEntity)
        {
            await CheckDuplicateSkillsNameAsync(newEntity).ConfigureAwait(false);
            _unitOfWork.GetRepository<Skills>().Update(newEntity);
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Skills, object> IncludeNavigationProperties(
            IQueryable<Skills> query
        )
        {
            return query.Include(x => x.UserSkills).ThenInclude(x => x.User);
        }

        // /// <summary>
        // /// Check if entity could be deleted
        // /// </summary>
        // /// <param name="entityToDelete">The entity being deleted</param>
        // protected override async Task CheckDeletePermissionsAsync(Skills entityToDelete, Guid CurrentUserId)
        // {
        //     var existSkill = await _unitOfWork
        //         .GetRepository<User>()
        //         .ExistsAsync(predicate: user => user.Skills.Any(skill => skill.Id == entityToDelete.Id))
        //         .ConfigureAwait(false);

        //     if (existSkill)
        //     {
        //         _logger.LogWarning(
        //             "Skills with id: {id} is assigned to User so it cannot be deleted for Skill with id: {Id}.",
        //             entityToDelete.Id,
        //             CurrentUserId
        //         );
        //         throw new ForbiddenException(_localizer.GetString("SkillCannotBeDeletedAssignedToUser"));
        //     }
        // }
        #endregion Protected Methods

        #region Private Methods
        /// <summary>
        /// Check duplicate name
        /// /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        private async Task CheckDuplicateSkillsNameAsync(Skills entity)
        {
            var skillsExist = await _unitOfWork
                .GetRepository<Skills>()
                .ExistsAsync(
                    predicate: p =>
                        p.Id != entity.Id && p.SkillName.ToLower() == entity.SkillName.ToLower()
                )
                .ConfigureAwait(false);
            if (skillsExist)
            {
                _logger.LogWarning(
                    "Duplicate Skills name : {name} is found for the Skills with id : {id}.",
                    entity.SkillName,
                    entity.Id
                );
                throw new ServiceException(_localizer.GetString("DuplicateSkillsNameFound"));
            }
        }

        #endregion Private Methods

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Skills, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }

        /// <summary>
        /// /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Skills, bool>> ConstructQueryConditions(
            Expression<Func<Skills, bool>> predicate,
            SkillsBaseSearchCriteria criteria
        )
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.SkillName.ToLower().Trim().Contains(search));
            }

            if (criteria.IsActive.HasValue)
            {
                predicate = predicate.And(p => p.IsActive == criteria.IsActive);
            }
            return predicate;
        }
    }
}
