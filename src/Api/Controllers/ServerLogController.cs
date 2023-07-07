using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lingtren.Api.Controllers
{

    public class ServerLogController : BaseApiController
    {
        private readonly ILogsService _logService;
        public ServerLogController(ILogsService logsService)
        {
            _logService = logsService;
        }

        /// <summary>
        /// get server logs api
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="LogBaseSearchCriteria"/></param>
        /// <returns> the instance of <see cref="ServerLogsResponseModel"/></returns>
        [HttpGet("logs")]
        public async Task<SearchResult<ServerLogsResponseModel>> Logs([FromQuery] LogBaseSearchCriteria criteria) => await _logService.GetServerLogsAsync(criteria, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get log details api
        /// </summary>
        /// <param name="logId"> the log id </param>
        /// <returns> the instance of <see cref="ServerLogsResponseModel"/></returns>
        [HttpGet("{logId}")]
        public async Task<ServerLogsResponseModel> Log(Guid logId) => await _logService.GetLogDetailAsync(logId,CurrentUser.Id).ConfigureAwait(false);

    }
}
