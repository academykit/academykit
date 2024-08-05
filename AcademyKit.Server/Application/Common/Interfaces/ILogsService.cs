using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Models.ResponseModels;

namespace AcademyKit.Application.Common.Interfaces
{
    public interface ILogsService
    {
        /// <summary>
        /// Handle to get server logs async
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="LogBaseSearchCriteria"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="ServerLogsResponseModel"/></returns>
        Task<SearchResult<ServerLogsResponseModel>> GetServerLogsAsync(
            LogBaseSearchCriteria criteria,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to get log details async
        /// </summary>
        /// <param name="logId"> the log id </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="ServerLogsResponseModel"/></returns>
        Task<ServerLogsResponseModel> GetLogDetailAsync(Guid logId, Guid currentUserId);
    }
}
