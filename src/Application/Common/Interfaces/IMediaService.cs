namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Http;

    public interface IMediaService
    {
        /// <summary>
        /// Handle to upload the file
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the instance of <see cref="FileResponseModel" />. </returns>
        Task<FileResponseModel> UploadFileAsync(MediaRequestModel model);

        /// <summary>
        /// Handle to upload group file
        /// </summary>
        /// <param name="file"> the instance of <see cref="IFormFile" /> .</param>
        /// <returns> the instance of <see cref="GroupFileDto" /> .</returns>
        Task<GroupFileDto> UploadGroupFileAsync(IFormFile file);

        /// <summary>
        /// Handle to update storage setting
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageSetting" /> . </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        Task<StorageSettingResponseModel> StorageUpdateSettingAsync(StorageSettingRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to get storage setting
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        Task<IList<StorageSettingResponseModel>> GetStorageSettingAsync(Guid currentUserId);

        /// <summary>
        /// Handle to upload the recording file
        /// </summary>
        /// <param name="fileUrl"> the file url </param>
        /// <param name="downloadToken"> the download token </param>
        /// <returns> the instance of <see cref="MediaFileDto" /> .</returns>
        Task<MediaFileDto> UploadRecordingFileAsync(string fileUrl, string downloadToken);

        /// <summary>
        /// Handle to get file
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the pre-signed url </returns>
        Task<string> GetFileAsnc(string key);
    }
}