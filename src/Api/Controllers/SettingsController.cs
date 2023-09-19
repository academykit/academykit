// <copyright file="SettingsController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/admin/settings")]
    public class SettingsController : BaseApiController
    {
        private readonly ILogger<SettingsController> logger;
        private readonly IGeneralSettingService generalSettingService;
        private readonly IZoomSettingService zoomSettingService;
        private readonly ISMTPSettingService smtpSettingService;
        private readonly IFileServerService fileServerService;
        private readonly IValidator<GeneralSettingRequestModel> generalSettingValidator;
        private readonly IValidator<ZoomSettingRequestModel> zoomSettingValidator;
        private readonly IValidator<SMTPSettingRequestModel> smtpSettingValidator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public SettingsController(
            ILogger<SettingsController> logger,
            IGeneralSettingService generalSettingService,
            IZoomSettingService zoomSettingService,
            ISMTPSettingService smtpSettingService,
            IFileServerService fileServerService,
            IValidator<GeneralSettingRequestModel> generalSettingValidator,
            IValidator<ZoomSettingRequestModel> zoomSettingValidator,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IValidator<SMTPSettingRequestModel> smtpSettingValidator)
        {
            this.logger = logger;
            this.generalSettingService = generalSettingService;
            this.zoomSettingService = zoomSettingService;
            this.smtpSettingService = smtpSettingService;
            this.fileServerService = fileServerService;
            this.generalSettingValidator = generalSettingValidator;
            this.zoomSettingValidator = zoomSettingValidator;
            this.smtpSettingValidator = smtpSettingValidator;
            this.localizer = localizer;
        }

        /// <summary>
        /// get general setting.
        /// </summary>
        /// <returns> the instance of <see cref="GeneralSettingResponseModel" /> .</returns>
        [HttpGet]
        public async Task<GeneralSettingResponseModel> Get()
        {
            var model = await generalSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            return new GeneralSettingResponseModel(model);
        }

        /// <summary>
        /// get company info api.
        /// </summary>
        /// <returns> the instance of <see cref="CompanyResponseModel" /> .</returns>
        [HttpGet("company")]
        [AllowAnonymous]
        public async Task<CompanyResponseModel> Company()
        {
            var response = await generalSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            return new CompanyResponseModel
            {
                Name = response.CompanyName,
                ImageUrl = response.LogoUrl,
                CustonConfiguration = response.CustomConfiguration,
            };
        }

        /// <summary>
        /// update general settings.
        /// </summary>
        /// <param name="id"> the general setting id.</param>
        /// <param name="model"> the  instance of <see cref="GeneralSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="GeneralSettingResponseModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<GeneralSettingResponseModel> UpdateGeneralSettings(Guid id, GeneralSettingRequestModel model)
        {
            IsSuperAdmin(CurrentUser.Role);

            await generalSettingValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await generalSettingService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);

            if (existing == null)
            {
                logger.LogWarning("General setting with id : {id} was not found.", id);
                throw new EntityNotFoundException(localizer.GetString("GeneralSettingNotFound"));
            }

            var currentTimeStamp = DateTime.UtcNow;

            var logoUrlKey = existing.LogoUrl;

            existing.Id = existing.Id;
            existing.LogoUrl = model.LogoUrl;
            existing.CompanyName = model.CompanyName;
            existing.CompanyAddress = model.CompanyAddress;
            existing.CompanyContactNumber = model.CompanyContactNumber;
            existing.EmailSignature = model.EmailSignature;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;
            existing.CustomConfiguration = model.CustomConfiguration;

            var savedEntity = await generalSettingService.UpdateAsync(existing).ConfigureAwait(false);

            if (logoUrlKey != model.LogoUrl && !string.IsNullOrWhiteSpace(logoUrlKey))
            {
                if (logoUrlKey.ToLower().Trim().Contains("/public/") && logoUrlKey.IndexOf("/standalone/") != -1)
                {
                    logoUrlKey = logoUrlKey.Substring(logoUrlKey.IndexOf("/standalone/") + "/standalone/".Length);
                }

                await fileServerService.RemoveFileAsync(logoUrlKey).ConfigureAwait(false);
            }

            return new GeneralSettingResponseModel(savedEntity);
        }

        /// <summary>
        /// get zoom setting.
        /// </summary>
        /// <returns> the instance of <see cref="ZoomSettingResponseModel" /> .</returns>
        [HttpGet("zoom")]
        public async Task<ZoomSettingResponseModel> GetZoomSetting()
        {
            IsSuperAdmin(CurrentUser.Role);

            var model = await zoomSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            return new ZoomSettingResponseModel(model);
        }

        /// <summary>
        /// update zoom settings.
        /// </summary>
        /// <param name="id"> the zoom setting id.</param>
        /// <param name="model"> the  instance of <see cref="ZoomSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="ZoomSettingResponseModel" /> .</returns>
        [HttpPut("zoom/{id}")]
        public async Task<ZoomSettingResponseModel> UpdateZoomSetting(Guid id, ZoomSettingRequestModel model)
        {
            IsSuperAdmin(CurrentUser.Role);

            await zoomSettingValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await zoomSettingService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);

            if (existing == null)
            {
                logger.LogWarning("Zoom setting with id : {id} was not found.", id);
                throw new EntityNotFoundException(localizer.GetString("ZoomSettingNotFound"));
            }

            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.SdkKey = model.SdkKey;
            existing.SdkSecret = model.SdkSecret;
            existing.WebHookSecret = model.WebhookSecret;
            existing.IsRecordingEnabled = model.IsRecordingEnabled;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;
            existing.OAuthAccountId = model.OAuthAccountId;
            existing.OAuthClientId = model.OAuthClientId;
            existing.OAuthClientSecret = model.OAuthClientSecret;

            var savedEntity = await zoomSettingService.UpdateAsync(existing).ConfigureAwait(false);
            return new ZoomSettingResponseModel(savedEntity);
        }

        /// <summary>
        /// get SMTP setting.
        /// </summary>
        /// <returns> the instance of <see cref="SMTPSettingResponseModel" /> .</returns>
        [HttpGet("smtp")]
        public async Task<SMTPSettingResponseModel> GetSMTPSetting()
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var model = await smtpSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            return new SMTPSettingResponseModel(model);
        }

        /// <summary>
        /// update SMTP settings.
        /// </summary>
        /// <param name="id"> the SMTP setting id.</param>
        /// <param name="model"> the  instance of <see cref="SMTPSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="SMTPSettingResponseModel" /> .</returns>
        [HttpPut("smtp/{id}")]
        public async Task<SMTPSettingResponseModel> UpdateSMTPSetting(Guid id, SMTPSettingRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await smtpSettingValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await smtpSettingService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);

            if (existing == null)
            {
                logger.LogWarning("SMTP setting with id : {id} was not found.", id);
                throw new EntityNotFoundException(localizer.GetString("SMTPSettingNotFound"));
            }

            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.MailPort = model.MailPort;
            existing.MailServer = model.MailServer;
            existing.ReplyTo = model.ReplyTo;
            existing.SenderName = model.SenderName;
            existing.SenderEmail = model.SenderEmail;
            existing.UserName = model.UserName;
            existing.ReplyTo = model.ReplyTo;
            existing.Password = model.Password;
            existing.UseSSL = model.UseSSL;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await smtpSettingService.UpdateAsync(existing).ConfigureAwait(false);
            return new SMTPSettingResponseModel(savedEntity);
        }
    }
}
