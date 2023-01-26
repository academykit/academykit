namespace Lingtren.Application.Common.Interfaces
{
    using System;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;

    public interface IFileServerService
    {
        /// <summary>
        /// Handle to upload the file async
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file key or url.</returns>
        Task<string> UploadFileAsync(MediaRequestModel model);

        /// <summary>
        /// Handle to upload the file path
        /// </summary>
        /// <param name="filePath"> the file path </param>
        /// <returns> the new file path </returns>
        Task<string> UploadRecordingFileAsync(string filePath,int fileSize);

        /// <summary>
        /// Handle to get file presigned url
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the pre signed url </returns>
        Task<string> GetFilePresignedUrl(string key);

        Task RemoveFileAsync(string key);
    }
}