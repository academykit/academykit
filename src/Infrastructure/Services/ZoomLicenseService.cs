namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
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
            if (criteria.IsActive != null)
            {
                predicate.And(p => p.IsActive == criteria.IsActive);
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
        protected override void SetDefaultSortOption(ZoomLicenseBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(ZoomLicense.CreatedOn);
            criteria.SortType = SortType.Descending;
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

        /// <summary>
        /// Handle to get active available zoom license with in given time period
        /// </summary>
        /// <param name="startDateTime">meeting start date</param>
        /// <param name="duration">meeting duration</param>
        /// <returns></returns>
        public async Task<IList<ZoomLicenseResponseModel>> GetActiveLicenses(DateTime startDateTime, int duration)
        {
            var data = from zoomLicense in _unitOfWork.DbContext.ZoomLicenses.ToList()
                       join meeting in _unitOfWork.DbContext.Meetings.ToList() on zoomLicense.Id equals meeting.ZoomLicenseId
                       where meeting.StartDate.HasValue == true && zoomLicense.IsActive ==true &&
                       (meeting.StartDate.Value.AddSeconds(meeting.Duration) < startDateTime || meeting.StartDate.Value > startDateTime.AddSeconds(duration))
                       group meeting by zoomLicense into g
                       select new
                       {
                           Id = g.Key.Id,
                           HostId = g.Key.HostId,
                           Capacity = g.Key.Capacity,
                           LicenseEmail = g.Key.LicenseEmail,
                           IsActive = g.Key.IsActive,
                           Count = g.Count()
                       };
            var response = data.Where(x => x.Count < 2).Select(x => new ZoomLicenseResponseModel
            {
                Id = x.Id,
                HostId = x.HostId,
                Capacity = x.Capacity,
                LicenseEmail = x.LicenseEmail,
                IsActive = x.IsActive,
            }).ToList();
            return await Task.FromResult(response);
        }

    }
}
