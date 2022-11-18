namespace Lingtren.Application.Common.Interfaces
{
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
    }
}