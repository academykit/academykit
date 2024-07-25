namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class MediaController : BaseApiController
    {
        private readonly IMediaService mediaService;
        private readonly IValidator<MediaRequestModel> validator;
        private readonly IStringLocalizer<ValidatorLocalizer> localizer;

        public MediaController(
            IMediaService mediaService,
            IValidator<MediaRequestModel> validator,
            IStringLocalizer<ValidatorLocalizer> localizer
        )
        {
            this.mediaService = mediaService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// update setting api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> . </returns>
        [HttpPut("setting")]
        public async Task<StorageSettingResponseModel> Update(StorageSettingRequestModel model)
        {
            if (model.Values.Any(x => x.Value == null))
            {
                throw new ForbiddenException(localizer.GetString("ValueCannotBeNullOrEmpty"));
            }

            var response = await mediaService
                .StorageUpdateSettingAsync(model, CurrentUser.Id)
                .ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// get storage setting api.
        /// </summary>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        [HttpGet("setting")]
        public async Task<IList<StorageSettingResponseModel>> Setting() =>
            await mediaService.GetStorageSettingAsync(CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// upload file api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the key or url. </returns>
        [HttpPost("file")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<string> File([FromForm] MediaRequestModel model)
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await mediaService.UploadFileAsync(model).ConfigureAwait(false);
        }

        /// <summary>
        /// get file api.
        /// </summary>
        /// <param name="key"> the file key. </param>
        /// <returns> the presigned url. </returns>
        [HttpGet("file")]
        public async Task<string> GetFile([FromQuery] string key) =>
            await mediaService.GetFileAsync(key).ConfigureAwait(false);
    }
}
