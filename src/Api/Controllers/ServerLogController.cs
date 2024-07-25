namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    public class ServerLogController : BaseApiController
    {
        private readonly ILogsService logService;

        public ServerLogController(ILogsService logsService)
        {
            logService = logsService;
        }

        /// <summary>
        /// get server logs api.
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="LogBaseSearchCriteria"/>.</param>
        /// <returns> the instance of <see cref="ServerLogsResponseModel"/>.</returns>
        [HttpGet("logs")]
        public async Task<SearchResult<ServerLogsResponseModel>> Logs(
            [FromQuery] LogBaseSearchCriteria criteria
        ) => await logService.GetServerLogsAsync(criteria, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get log details api.
        /// </summary>
        /// <param name="logId"> the log id. </param>
        /// <returns> the instance of <see cref="ServerLogsResponseModel"/>.</returns>
        [HttpGet("{logId}")]
        public async Task<ServerLogsResponseModel> Log(Guid logId) =>
            await logService.GetLogDetailAsync(logId, CurrentUser.Id).ConfigureAwait(false);
    }
}
