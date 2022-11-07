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

    public class ZoomLicenseService : BaseGenericService<ZoomLicense, ZoomLicenseBaseSearchCriteria>, IZoomLicenseService
    {
        public ZoomLicenseService(
            IUnitOfWork unitOfWork,
            ILogger<ZoomLicenseService> logger) : base(unitOfWork, logger)
        {
        }

        #region Protected Methods
        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<ZoomLicense, bool>> ConstructQueryConditions(Expression<Func<ZoomLicense, bool>> predicate, ZoomLicenseBaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.LicenseEmail.ToLower().Trim().Contains(search)
                || x.HostId.Contains(search));
            }

            return predicate.And(p => p.IsActive == criteria.IsActive);
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(ZoomLicenseBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(ZoomLicense.CreatedOn);
            criteria.SortType = SortType.Ascending;
        }


        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<ZoomLicense, object> IncludeNavigationProperties(IQueryable<ZoomLicense> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<ZoomLicense, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }
        #endregion Protected Methods

    }
}
