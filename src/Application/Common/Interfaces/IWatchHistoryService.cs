﻿namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;

    public interface IWatchHistoryService : IGenericService<WatchHistory, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to save watch history
        /// </summary>
        /// <param name="model">the instance of <see cref="WatchHistoryRequestModel"/> </param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task<WatchHistoryResponseModel> CreateAsync(
            WatchHistoryRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to pass student in requested lesson
        /// </summary>
        /// <param name="userId">the requested user id</param>
        /// <param name="model">the instance of <see cref="WatchHistoryRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task PassAsync(Guid userId, WatchHistoryRequestModel model, Guid currentUserId);
    }
}
