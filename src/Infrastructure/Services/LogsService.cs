using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Lingtren.Infrastructure.Services
{
    public class LogsService : BaseService, ILogsService
    {
        public LogsService(IUnitOfWork unitOfWork, ILogger<LogsService> logger, IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
        }


        /// <summary>
        /// Handle to get server logs async
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="LogBaseSearchCriteria"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="ServerLogsResponseModel"/></returns>
        public async Task<SearchResult<ServerLogsResponseModel>> GetServerLogsAsync(LogBaseSearchCriteria criteria, Guid currentUserId)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var hasAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var response = new SearchResult<ServerLogsResponseModel>
                {
                    Items = new List<ServerLogsResponseModel>(),
                    CurrentPage = 1,
                    PageSize = 1,
                    TotalCount = 1,
                };
                return response;
            });
        }

        /// <summary>
        /// Handle to get log details async
        /// </summary>
        /// <param name="logId"> the log id </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="ServerLogsResponseModel"/></returns>
        public async Task<ServerLogsResponseModel> GetLogDetailAsync(Guid logId, Guid currentUserId)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var hasAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }
                return new ServerLogsResponseModel();
            });
         }
    }
}
