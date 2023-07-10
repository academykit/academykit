namespace Lingtren.Api.Controllers
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Application.ValidatorLocalization;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Localization;
    using System.Net;

    public class MediaController : BaseApiController
    {
        private readonly IMediaService _mediaService;
        private readonly IValidator<MediaRequestModel> _validator;
        private readonly ILogger<MediaController> _logger;
        private readonly IStringLocalizer<ValidatorLocalizer> _localizer;
        public MediaController(
            IMediaService mediaService,
            IValidator<MediaRequestModel> validator,
            ILogger<MediaController> logger,
            IStringLocalizer<ValidatorLocalizer> localizer)
        {
            _mediaService = mediaService;
            _validator = validator;
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// update setting api
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> . </returns>
        [HttpPut("setting")]
        public async Task<StorageSettingResponseModel> Update(StorageSettingRequestModel model)
        {
            if (model.Values.Any(x => x.Value == null))
            {
                throw new ForbiddenException(_localizer.GetString("ValueCannotBeNullOrEmpty"));
            }
            var response = await _mediaService.StorageUpdateSettingAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// get storage setting api
        /// </summary>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        [HttpGet("setting")]
        public async Task<IList<StorageSettingResponseModel>> Setting() => await _mediaService.GetStorageSettingAsync(CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// upload file api
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the key or url </returns>
        [HttpPost("file")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<string> File([FromForm] MediaRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            return await _mediaService.UploadFileAsync(model).ConfigureAwait(false);
        }

        /// <summary>
        /// get file api
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the presigned url </returns>
        [HttpGet("file")]
        public async Task<string> GetFile([FromQuery] string key) => await _mediaService.GetFileAsnc(key).ConfigureAwait(false);
    }
}