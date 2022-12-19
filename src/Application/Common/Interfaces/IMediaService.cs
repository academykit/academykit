namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Enums;
    using Microsoft.AspNetCore.Http;

    public interface IMediaService
    {
        /// <summary>
        /// Handle to upload the file
        /// </summary>
        /// <param name="file"> the instance of <see cref="file" /> .</param>
        /// <returns> the file url </returns>
        Task<string> UploadFile(IFormFile file);

        /// <summary>
        /// Handle to upload video file
        /// </summary>
        /// <param name="file"> the instance of <see cref="file" /> .</param>
        /// <returns> the file url </returns>
        Task<string> UploadVideo(IFormFile file);

        /// <summary>
        /// Handle to update storage setting
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageSetting" /> . </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        Task<StorageSettingResponseModel> StorageUpdateSettingAsync(StorageSettingRequestModel model,Guid currentUserId);

        /// <summary>
        /// Handle to get storage setting
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        Task<StorageSettingResponseModel> GetStorageSettingAsync(Guid currentUserId);

        /// <summary>
        /// Handle to get setting values
        /// </summary>
        /// <param name="type"> the instance of <see cref="StorageType" />. </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="SettingValue" /> .</returns>
        Task<IList<SettingValue>> GetSettingValuesAsync(StorageType type, Guid currentUserId);
    }
}