namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;

    public class GroupService : BaseGenericService<Group, BaseSearchCriteria>, IGroupService
    {
        public GroupService(IUnitOfWork unitOfWork,
            ILogger<GroupService> logger) : base(unitOfWork, logger)
        {

        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Group, bool>> ConstructQueryConditions(Expression<Func<Group, bool>> predicate, BaseSearchCriteria criteria)
        {

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
            }

            return predicate.And(x => x.CreatedBy == criteria.CurrentUserId);
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
            criteria.SortBy = nameof(Group.CreatedOn);
            criteria.SortType = SortType.Ascending;
        }


        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Group, object> IncludeNavigationProperties(IQueryable<Group> query)
        {
            return query
                .Include(x => x.User);
        }
    }
}
