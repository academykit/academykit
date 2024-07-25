namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class ZoomSettingService
        : BaseGenericService<ZoomSetting, BaseSearchCriteria>,
            IZoomSettingService
    {
        public ZoomSettingService(
            IUnitOfWork unitOfWork,
            ILogger<ZoomSettingService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<ZoomSetting, object> IncludeNavigationProperties(
            IQueryable<ZoomSetting> query
        )
        {
            return query.Include(x => x.User);
        }
    }
}
