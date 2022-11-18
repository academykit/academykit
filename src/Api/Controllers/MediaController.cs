namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Microsoft.AspNetCore.Mvc;

    public class MediaController : BaseApiController
    {
        private readonly IMediaService _mediaService;
        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        /// <summary>
        /// upload file api
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file url </returns>
        [HttpPost]
        public async Task<string> Upload([FromForm] MediaRequestModel model)
        {
            var fileUrl = await _mediaService.UploadFile(model.File).ConfigureAwait(false);
            return fileUrl;
        }

        /// <summary>
        /// upload video api
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the video url </returns>
        [HttpPost("video")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<string> Video([FromForm] MediaRequestModel model)
        {
            var fileUrl = await _mediaService.UploadVideo(model.File).ConfigureAwait(false);
            return fileUrl;
        }
    }
}