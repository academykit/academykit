using System.Reflection;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Api.Controllers;

[Route("api/admin/settings")]
public class SettingsController : BaseApiController
{
    private readonly ILogger<SettingsController> logger;
    private readonly IGeneralSettingService generalSettingService;
    private readonly IZoomSettingService zoomSettingService;
    private readonly ISMTPSettingService smtpSettingService;
    private readonly IFileServerService fileServerService;
    private readonly ISettingService _settingService;
    private readonly IValidator<GeneralSettingRequestModel> generalSettingValidator;
    private readonly IValidator<ZoomSettingRequestModel> zoomSettingValidator;
    private readonly IValidator<SMTPSettingRequestModel> smtpSettingValidator;
    private readonly IValidator<InitialSetupRequestModel> _initialSetupValidator;
    private readonly IStringLocalizer<ExceptionLocalizer> localizer;
    private readonly IConfiguration configuration;

    public SettingsController(
        ILogger<SettingsController> logger,
        IGeneralSettingService generalSettingService,
        IZoomSettingService zoomSettingService,
        ISMTPSettingService smtpSettingService,
        IFileServerService fileServerService,
        ISettingService settingService,
        IValidator<GeneralSettingRequestModel> generalSettingValidator,
        IValidator<ZoomSettingRequestModel> zoomSettingValidator,
        IValidator<SMTPSettingRequestModel> smtpSettingValidator,
        IValidator<InitialSetupRequestModel> initialSetupValidator,
        IStringLocalizer<ExceptionLocalizer> localizer,
        IConfiguration configuration
    )
    {
        this.logger = logger;
        this.generalSettingService = generalSettingService;
        this.zoomSettingService = zoomSettingService;
        this.smtpSettingService = smtpSettingService;
        this.fileServerService = fileServerService;
        _settingService = settingService;
        this.generalSettingValidator = generalSettingValidator;
        this.zoomSettingValidator = zoomSettingValidator;
        this.smtpSettingValidator = smtpSettingValidator;
        _initialSetupValidator = initialSetupValidator;
        this.localizer = localizer;
        this.configuration = configuration;
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
            Name = response?.CompanyName,
            ImageUrl = response?.LogoUrl,
            CustomConfiguration = response?.CustomConfiguration,
            IsSetupCompleted = response?.IsSetupCompleted,
            AppVersion = Assembly
                .GetEntryAssembly()
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion,
        };
    }

    /// <summary>
    /// update general settings.
    /// </summary>
    /// <param name="id"> the general setting id.</param>
    /// <param name="model"> the  instance of <see cref="GeneralSettingRequestModel" /> .</param>
    /// <returns> the instance of <see cref="GeneralSettingResponseModel" /> .</returns>
    [HttpPut("{id}")]
    public async Task<GeneralSettingResponseModel> UpdateGeneralSettings(
        Guid id,
        GeneralSettingRequestModel model
    )
    {
        IsSuperAdmin(CurrentUser.Role);

        await generalSettingValidator
            .ValidateAsync(model, options => options.ThrowOnFailures())
            .ConfigureAwait(false);
        var existing = await generalSettingService
            .GetAsync(id, CurrentUser.Id)
            .ConfigureAwait(false);

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
            if (
                logoUrlKey.ToLower().Trim().Contains("/public/")
                && logoUrlKey.IndexOf("/standalone/") != -1
            )
            {
                logoUrlKey = logoUrlKey.Substring(
                    logoUrlKey.IndexOf("/standalone/") + "/standalone/".Length
                );
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
        return model != null ? new ZoomSettingResponseModel(model) : new ZoomSettingResponseModel();
    }

    /// <summary>
    /// Create or update zoom settings.
    /// </summary>
    /// <param name="model">The instance of <see cref="ZoomSettingRequestModel"/>.</param>
    /// <returns>The instance of <see cref="ZoomSettingResponseModel"/>.</returns>
    [HttpPost("zoom")]
    public async Task<ZoomSettingResponseModel> CreateUpdateZoomSetting(
        ZoomSettingRequestModel model
    )
    {
        IsSuperAdmin(CurrentUser.Role);

        await zoomSettingValidator
            .ValidateAsync(model, options => options.ThrowOnFailures())
            .ConfigureAwait(false);

        var existing = await zoomSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
        var currentTimeStamp = DateTime.UtcNow;

        var zoomSetting =
            existing
            ?? new ZoomSetting
            {
                Id = Guid.NewGuid(),
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp
            };

        zoomSetting.SdkKey = model.SdkKey;
        zoomSetting.SdkSecret = model.SdkSecret;
        zoomSetting.WebHookSecret = model.WebhookSecret;
        zoomSetting.IsRecordingEnabled = model.IsRecordingEnabled;
        zoomSetting.OAuthAccountId = model.OAuthAccountId;
        zoomSetting.OAuthClientId = model.OAuthClientId;
        zoomSetting.OAuthClientSecret = model.OAuthClientSecret;
        zoomSetting.UpdatedBy = CurrentUser.Id;
        zoomSetting.UpdatedOn = currentTimeStamp;

        var savedEntity =
            existing == null
                ? await zoomSettingService.CreateAsync(zoomSetting).ConfigureAwait(false)
                : await zoomSettingService.UpdateAsync(zoomSetting).ConfigureAwait(false);

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
        return model != null ? new SMTPSettingResponseModel(model) : new SMTPSettingResponseModel();
    }

    /// <summary>
    /// create or update SMTP settings.
    /// </summary>
    /// <param name="model"> the  instance of <see cref="SMTPSettingRequestModel" /> .</param>
    /// <returns> the instance of <see cref="SMTPSettingResponseModel" /> .</returns>
    [HttpPost("smtp")]
    public async Task<SMTPSettingResponseModel> CreateUpdateSMTPSetting(
        SMTPSettingRequestModel model
    )
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);

        await smtpSettingValidator
            .ValidateAsync(model, options => options.ThrowOnFailures())
            .ConfigureAwait(false);

        var existing = await smtpSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
        var currentTimeStamp = DateTime.UtcNow;

        var smtpSetting =
            existing
            ?? new SMTPSetting
            {
                Id = Guid.NewGuid(),
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp
            };

        smtpSetting.MailPort = model.MailPort;
        smtpSetting.MailServer = model.MailServer;
        smtpSetting.ReplyTo = model.ReplyTo;
        smtpSetting.SenderName = model.SenderName;
        smtpSetting.SenderEmail = model.SenderEmail;
        smtpSetting.UserName = model.UserName;
        smtpSetting.Password = model.Password;
        smtpSetting.UseSSL = model.UseSSL;
        smtpSetting.UpdatedBy = CurrentUser.Id;
        smtpSetting.UpdatedOn = currentTimeStamp;

        var savedEntity =
            existing == null
                ? await smtpSettingService.CreateAsync(smtpSetting).ConfigureAwait(false)
                : await smtpSettingService.UpdateAsync(smtpSetting).ConfigureAwait(false);

        return new SMTPSettingResponseModel(savedEntity);
    }

    /// <summary>
    /// get updates information.
    /// </summary>
    /// <returns> the instance of <see cref="CheckUpdatesResponseModel" /> .</returns>
    [HttpGet("CheckUpdates")]
    public async Task<CheckUpdatesResponseModel> CheckUpdates()
    {
        var registry = configuration.GetValue<string>("Docker:Registry");
        var repo = configuration.GetValue<string>("Docker:Repo");
        var releaseNotesUrl = configuration.GetValue<string>("Docker:ReleaseNotesUrl");

        var tags = await Infrastructure.Helpers.HttpClientUtils.GetImageTagsAsync(registry, repo);

        var latestRemoteVersion = Infrastructure.Helpers.CommonHelper.FilterLatestSemanticVersion(
            tags
        );
        var currentVersion = Assembly
            .GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        return new CheckUpdatesResponseModel
        {
            Latest = latestRemoteVersion,
            Current = currentVersion,
            Available =
                currentVersion == null
                || latestRemoteVersion == null
                || Infrastructure.Helpers.CommonHelper.CompareVersion(
                    currentVersion,
                    latestRemoteVersion
                ) < 0,
            ReleaseNotesUrl = releaseNotesUrl
        };
    }

    /// <summary>
    /// Retrieves the list of allowed domains.
    /// </summary>
    /// <returns>A string containing the allowed domains, separated by commas.</returns>
    [HttpGet("allowed-domains")]
    public async Task<ActionResult<string>> GetAllowedDomains()
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        var domains = await _settingService.GetAllowedDomainsAsync();
        return Ok(domains);
    }

    /// <summary>
    /// Updates the list of allowed domains.
    /// </summary>
    /// <param name="domains">A string containing the new allowed domains, separated by commas.</param>
    /// <returns>A string containing the updated list of allowed domains.</returns>
    [HttpPut("allowed-domains")]
    public async Task<ActionResult<string>> UpdateAllowedDomains([FromBody] string domains)
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        var updatedDomains = await _settingService.UpdateAllowedDomainsAsync(domains);
        return Ok(updatedDomains);
    }

    /// <summary>
    /// initial setup api
    /// </summary>
    /// <param name="model">the instance of <see cref="InitialSetupRequestModel"/></param>
    /// <returns>the instance of <see cref="GeneralSettingResponseModel"/></returns>
    [HttpPost("/api/initialSetup")]
    [AllowAnonymous]
    public async Task<GeneralSettingResponseModel> InitialSetup(InitialSetupRequestModel model)
    {
        await _initialSetupValidator
            .ValidateAsync(model, options => options.ThrowOnFailures())
            .ConfigureAwait(false);
        var savedEntity = await generalSettingService
            .InitialSetupAsync(model)
            .ConfigureAwait(false);

        return new GeneralSettingResponseModel(savedEntity);
    }
}
