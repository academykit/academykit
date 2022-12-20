using System.Reflection.Emit;
namespace Lingtren.Api.Controllers
{
    using Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    public class MediaController : BaseApiController
    {
        private readonly IMediaService _mediaService;
        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        /// <summary>
        /// update setting api
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> . </returns>
        [HttpPut("setting")]
        public async Task<StorageSettingResponseModel> Update(StorageSettingRequestModel model)
        {
            var response = await _mediaService.StorageUpdateSettingAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// get storage setting api
        /// </summary>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        [HttpGet("setting")]
        public async Task<StorageSettingResponseModel> Setting() => await _mediaService.GetStorageSettingAsync(CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get setting value api
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageTypeRequestModel" />.</param>
        /// <returns> the list of <see cref="SettingValue" /> </returns>
        [HttpGet("settingvalue")]
        public async Task<IList<SettingValue>> SettingValue([FromQuery] StorageTypeRequestModel model) =>
        await _mediaService.GetSettingValuesAsync(model.Type, CurrentUser.Id).ConfigureAwait(false);


        /// <summary>
        /// upload file api
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file url </returns>
        [HttpPost("file")]
        public async Task<string> Upload([FromForm] MediaRequestModel model) => await _mediaService.UploadFileAsync(model.File).ConfigureAwait(false);

        /// <summary>
        /// upload video api
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the video url </returns>
        [HttpPost("video")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<string> Video([FromForm] MediaRequestModel model) => await _mediaService.UploadVideoAsync(model.File).ConfigureAwait(false);
    }
}