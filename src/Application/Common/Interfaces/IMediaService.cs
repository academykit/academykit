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
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file url </returns>
        Task<string> UploadFileAsync(MediaRequestModel model);

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
        Task<IList<StorageSettingResponseModel>> GetStorageSettingAsync(Guid currentUserId);
    }
}