namespace AcademyKit.Infrastructure.Services
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class SMTPSettingService
        : BaseGenericService<SMTPSetting, BaseSearchCriteria>,
            ISMTPSettingService
    {
        public SMTPSettingService(
            IUnitOfWork unitOfWork,
            ILogger<SMTPSettingService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<SMTPSetting, object> IncludeNavigationProperties(
            IQueryable<SMTPSetting> query
        )
        {
            return query.Include(x => x.User);
        }
    }
}
