namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;

    public class GeneralSettingService : BaseGenericService<GeneralSetting, BaseSearchCriteria>, IGeneralSettingService
    {
        public GeneralSettingService(
            IUnitOfWork unitOfWork,
            ILogger<GeneralSettingService> logger) : base(unitOfWork, logger)
        {

        }
        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<GeneralSetting, object> IncludeNavigationProperties(IQueryable<GeneralSetting> query)
        {
            return query.Include(x => x.User);
        }
    }
}
