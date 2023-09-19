namespace Lingtren.Infrastructure.Services
{
    using System.Linq.Expressions;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class GroupMemberService : BaseGenericService<GroupMember, GroupMemberBaseSearchCriteria>, IGroupMemberService
    {
        public GroupMemberService(
            IUnitOfWork unitOfWork,
            ILogger<GroupMemberService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<GroupMember, bool>> ConstructQueryConditions(Expression<Func<GroupMember, bool>> predicate, GroupMemberBaseSearchCriteria criteria)
        {
            predicate = predicate.And(p => p.GroupId == criteria.GroupId && p.IsActive);
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => ((x.User.FirstName.Trim() + " " + x.User.MiddleName.Trim()).Trim() + " " + x.User.LastName.Trim()).Trim().Contains(search) ||
                x.User.Email.ToLower().Trim().Contains(search));
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
        protected override void SetDefaultSortOption(GroupMemberBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Group.CreatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<GroupMember, object> IncludeNavigationProperties(IQueryable<GroupMember> query)
        {
            return query
                .Include(x => x.Group)
                .Include(x => x.User);
        }
    }
}
