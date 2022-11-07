using FluentValidation;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lingtren.Api.Controllers
{
    [Route("api/admin/settings")]
    public class SettingsController : BaseApiController
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IGeneralSettingService _generalSettingService;
        private readonly IZoomSettingService _zoomSettingService;
        private readonly ISMTPSettingService _smtpSettingService;
        private readonly IValidator<GeneralSettingRequestModel> _generalSettingValidator;
        private readonly IValidator<ZoomSettingRequestModel> _zoomSettingValidator;
        private readonly IValidator<SMTPSettingRequestModel> _smtpSettingValidator;

        public SettingsController(
            ILogger<SettingsController> logger,
            IGeneralSettingService generalSettingService,
            IZoomSettingService zoomSettingService,
            ISMTPSettingService smtpSettingService,
            IValidator<GeneralSettingRequestModel> generalSettingValidator,
            IValidator<ZoomSettingRequestModel> zoomSettingValidator,
            IValidator<SMTPSettingRequestModel> smtpSettingValidator)
        {
            _logger = logger;
            _generalSettingService = generalSettingService;
            _zoomSettingService = zoomSettingService;
            _smtpSettingService = smtpSettingService;
            _generalSettingValidator = generalSettingValidator;
            _zoomSettingValidator = zoomSettingValidator;
            _smtpSettingValidator = smtpSettingValidator;
        }

        #region general settings

        /// <summary>
        /// get general  setting by id
        /// </summary>
        /// <param name="id"> the general setting id</param>
        /// <returns> the instance of <see cref="GeneralSettingResponseModel" /> .</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<GeneralSettingResponseModel> Get()
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with id : {id} is not admin to access general setting", CurrentUser.Id);
                throw new ForbiddenException("Only user with admin role is allowed to view general setting");
            }
            var model = await _generalSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            return new GeneralSettingResponseModel(model);
        }

        /// <summary>
        /// update general settings
        /// </summary>
        /// <param name="id"> the general setting id</param>
        /// <param name="model"> the  instance of <see cref="GeneralSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="GeneralSettingResponseModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<GeneralSettingResponseModel> UpdateSMTPSetting(Guid id, GeneralSettingRequestModel model)
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} is not allowed to edit general setting with Id : {generalSettingId}", CurrentUser.Id, id);
                throw new ForbiddenException("Only user with admin role is allowed to update general setting");
            }

            await _generalSettingValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _generalSettingService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);

            if (existing == null)
            {
                _logger.LogWarning("General Setting with id : {id} is not found", id);
                throw new EntityNotFoundException("General setting was not found.");
            }
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.LogoUrl = model.LogoUrl;
            existing.CompanyName = model.CompanyName;
            existing.CompanyAddress = model.CompanyAddress;
            existing.CompanyContactNumber = model.CompanyContactNumber;
            existing.EmailSignature = model.EmailSignature;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _generalSettingService.UpdateAsync(existing).ConfigureAwait(false);
            return new GeneralSettingResponseModel(savedEntity);
        }

        #endregion general settings

        #region zoom settings

        /// <summary>
        /// get zoom  setting by id
        /// </summary>
        /// <returns> the instance of <see cref="ZoomSettingResponseModel" /> .</returns>
        [HttpGet("zoom")]
        public async Task<ZoomSettingResponseModel> GetZoomSetting()
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with id : {id} is not admin to access zoom setting", CurrentUser.Id);
                throw new ForbiddenException("Only user with admin role is allowed to view zoom setting");
            }
            var model = await _zoomSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            return new ZoomSettingResponseModel(model);
        }

        /// <summary>
        /// update zoom settings
        /// </summary>
        /// <param name="id"> the zoom setting id</param>
        /// <param name="model"> the  instance of <see cref="ZoomSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="ZoomSettingResponseModel" /> .</returns>
        [HttpPut("zoom/{id}")]
        public async Task<ZoomSettingResponseModel> UpdateZoomSetting(Guid id, ZoomSettingRequestModel model)
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} is not allowed to edit zoom setting with Id : {zoomSettingId}", CurrentUser.Id, id);
                throw new ForbiddenException("Only user with admin role is allowed to update zoom setting");
            }

            await _zoomSettingValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _zoomSettingService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);

            if (existing == null)
            {
                _logger.LogWarning("Zoom Setting with id : {id} is not found", id);
                throw new EntityNotFoundException("Zoom setting was not found.");
            }
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.ApiKey = model.ApiKey;
            existing.SecretKey = model.SecretKey;
            existing.IsRecordingEnabled = model.IsRecordingEnabled;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _zoomSettingService.UpdateAsync(existing).ConfigureAwait(false);
            return new ZoomSettingResponseModel(savedEntity);
        }

        #endregion zoom settings

        #region smtp settings

        /// <summary>
        /// get smtp  setting by id
        /// </summary>
        /// <param name="id"> the smtp setting id</param>
        /// <returns> the instance of <see cref="SMTPSettingResponseModel" /> .</returns>
        [HttpGet("smtp")]
        [AllowAnonymous]
        public async Task<SMTPSettingResponseModel> GetSMTPSetting()
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with id : {id} is not admin to access smtp setting", CurrentUser.Id);
                throw new ForbiddenException("Only user with admin role is allowed to view smtp setting");
            }
            var model = await _smtpSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            return new SMTPSettingResponseModel(model);
        }

        /// <summary>
        /// update smtp settings
        /// </summary>
        /// <param name="id"> the smtp setting id</param>
        /// <param name="model"> the  instance of <see cref="SMTPSettingRequestModel" /> .</param>
        /// <returns> the instance of <see cref="SMTPSettingResponseModel" /> .</returns>
        [HttpPut("smtp/{id}")]
        public async Task<SMTPSettingResponseModel> UpdateSMTPSetting(Guid id, SMTPSettingRequestModel model)
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} is not allowed to edit smtp setting with Id : {smtpSettingId}", CurrentUser.Id, id);
                throw new ForbiddenException("Only user with admin role is allowed to update smtp setting");
            }

            await _smtpSettingValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _smtpSettingService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);

            if (existing == null)
            {
                _logger.LogWarning("SMTP Setting with id : {id} is not found", id);
                throw new EntityNotFoundException("SMTP setting was not found.");
            }
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.MailPort = model.MailPort;
            existing.MailServer = model.MailServer;
            existing.SenderName = model.SenderName;
            existing.SenderEmail = model.SenderEmail;
            existing.UserName = model.UserName;
            existing.Password = model.Password;
            existing.UseSSL = model.UseSSL;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _smtpSettingService.UpdateAsync(existing).ConfigureAwait(false);
            return new SMTPSettingResponseModel(savedEntity);
        }

        #endregion smtp settings


    }
}
