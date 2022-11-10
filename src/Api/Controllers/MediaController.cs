namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Microsoft.AspNetCore;
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
            var fileUrl =await _mediaService.UploadFile(model.File).ConfigureAwait(false);
            return fileUrl;
        }
    }
}