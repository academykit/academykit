﻿namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;

    public class SMTPSettingService : BaseGenericService<SMTPSetting, BaseSearchCriteria>, ISMTPSettingService
    {
        public SMTPSettingService(
            IUnitOfWork unitOfWork,
            ILogger<SMTPSettingService> logger) : base(unitOfWork, logger)
        {

        }
        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<SMTPSetting, object> IncludeNavigationProperties(IQueryable<SMTPSetting> query)
        {
            return query.Include(x => x.User);
        }
    }
}