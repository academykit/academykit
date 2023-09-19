namespace Lingtren.Application.Common.Interfaces
{
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
        Task<string> UploadRecordingFileAsync(string filePath, int fileSize);

        /// <summary>
        /// Handle to get file presigned url
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the pre signed url </returns>
        Task<string> GetFilePresignedUrl(string key);

        /// <summary>
        /// Handle to remove file
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the task complete </returns>
        Task RemoveFileAsync(string key);

        /// <summary>
        /// Handle to get file local path async
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the local file path </returns>
        Task<string> GetFileLocalPathAsync(string key);
    }
}
