namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    public interface IWatchHistoryService : IGenericService<WatchHistory, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to save watch history
        /// </summary>
        /// <param name="model">the instance of <see cref="WatchHistoryRequestModel"/> </param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task<WatchHistoryResponseModel> CreateAsync(WatchHistoryRequestModel model, Guid currentUserId);
    }
}