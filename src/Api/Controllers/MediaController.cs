namespace Lingtren.Api.Controllers
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.StaticFiles;
    using System.Net;

    public class MediaController : BaseApiController
    {
        private readonly IMediaService _mediaService;
        private readonly IValidator<MediaRequestModel> _validator;
        public MediaController(
            IMediaService mediaService,
            IValidator<MediaRequestModel> validator)
        {
            _mediaService = mediaService;
            _validator = validator;
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
        public async Task<IList<StorageSettingResponseModel>> Setting() => await _mediaService.GetStorageSettingAsync(CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// upload file api
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the video url </returns>
        [HttpPost("file")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<string> Video([FromForm] MediaRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            return await _mediaService.UploadFileAsync(model).ConfigureAwait(false);
        }

        [AllowAnonymous]
        [HttpGet("/read")]
        public async Task<IActionResult> Read()
        {
            try
            {

                //NetworkCredential credentials = new NetworkCredential(@"smbadmin", "smbadmin");
                string networkPath = @"\\159.89.163.233\public";
                //string myNetworkPath = string.Empty;
                //using (new ConnectToSharedFolder(networkPath, credentials))
                //{
                    var fileList = Directory.GetDirectories(networkPath);
                    var filePath = Path.Combine(networkPath, "hello.mp4");
                    var mimeType = GetMimeTypeForFileExtension(filePath);
                    return PhysicalFile(filePath, mimeType, enableRangeProcessing: true);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }
            return contentType;
        }
    }
}