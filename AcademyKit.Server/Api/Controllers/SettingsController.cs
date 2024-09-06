using System.Reflection;
using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Enums;
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
            AppVersion = GetAppVersion()
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
        await ValidateModelAsync(generalSettingValidator, model).ConfigureAwait(false);
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
    /// update zoom settings.
    /// </summary>
    /// <param name="id"> the zoom setting id.</param>
    /// <param name="model"> the  instance of <see cref="ZoomSettingRequestModel" /> .</param>
    /// <returns> the instance of <see cref="ZoomSettingResponseModel" /> .</returns>
    [HttpPut("zoom/{id}")]
    public async Task<ZoomSettingResponseModel> UpdateZoomSetting(
        Guid id,
        ZoomSettingRequestModel model
    )
    {
        IsSuperAdmin(CurrentUser.Role);
        await ValidateModelAsync(zoomSettingValidator, model).ConfigureAwait(false);
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
        return model != null ? new SMTPSettingResponseModel(model) : new SMTPSettingResponseModel();
    }

    /// <summary>
    /// update SMTP settings.
    /// </summary>
    /// <param name="id"> the SMTP setting id.</param>
    /// <param name="model"> the  instance of <see cref="SMTPSettingRequestModel" /> .</param>
    /// <returns> the instance of <see cref="SMTPSettingResponseModel" /> .</returns>
    [HttpPut("smtp/{id}")]
    public async Task<SMTPSettingResponseModel> UpdateSMTPSetting(
        Guid id,
        SMTPSettingRequestModel model
    )
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        await ValidateModelAsync(smtpSettingValidator, model).ConfigureAwait(false);
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

    /// <summary>
    /// get updates information.
    /// </summary>
    /// <returns> the instance of <see cref="CheckUpdatesResponseModel" /> .</returns>
    [HttpGet("checkUpdates")]
    public async Task<CheckUpdatesResponseModel> CheckUpdates()
    {
        var registry = configuration.GetValue<string>("Docker:Registry");
        var repo = configuration.GetValue<string>("Docker:Repo");
        var releaseNotesUrl = configuration.GetValue<string>("Docker:ReleaseNotesUrl");
        var tags = await Infrastructure.Helpers.HttpClientUtils.GetImageTagsAsync(registry, repo);
        var latestRemoteVersion = Infrastructure.Helpers.CommonHelper.FilterLatestSemanticVersion(
            tags
        );
        var currentVersion = GetAppVersion();

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
    [HttpGet("allowedDomains")]
    public async Task<string> GetAllowedDomains()
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        var domains = await _settingService.GetAllowedDomainsAsync();
        return domains;
    }

    /// <summary>
    /// Updates the list of allowed domains.
    /// </summary>
    /// <param name="domains">A string containing the new allowed domains, separated by commas.</param>
    /// <returns>A string containing the updated list of allowed domains.</returns>
    [HttpPost("allowedDomains")]
    public async Task<string> SetAllowedDomains([FromBody] string domains)
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        var savedDomains = await _settingService.SetAllowedDomainsAsync(domains);
        return savedDomains;
    }

    /// <summary>
    /// Retrieve the default user role
    /// </summary>
    /// <returns>the user role</returns>
    [HttpGet("defaultRole")]
    public async Task<UserRole> GetDefaultRole()
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        var defaultRole = await _settingService.GetDefaultRole();
        return defaultRole;
    }

    /// <summary>
    /// Create or Update the default user role
    /// </summary>
    /// <param name="role">the user role</param>
    /// <returns>the saved user role</returns>
    [HttpPost("defaultRole")]
    public async Task<UserRole> SetDefaultRole([FromBody] UserRole role)
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        var savedEntity = await _settingService.SetDefaultRole(role);
        return savedEntity;
    }

    /// <summary>
    /// Retrieves the list of sign in options
    /// </summary>
    /// <returns>the list of <see cref="SignInOptionDto"/></returns>
    [HttpGet("signInOptions")]
    public async Task<List<SignInOptionDto>> GetSignInOptions()
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        return await _settingService.GetSignInOptionsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Create or update signInOption api
    /// </summary>
    /// <param name="signInOption">the instance of <see cref="SignInOptionDto"/></param>
    /// <returns>the instance of <see cref="SignInOptionDto"/> </returns>
    [HttpPost("signInOptions")]
    public async Task<SignInOptionDto> SetSignInOptions([FromBody] SignInOptionDto signInOption)
    {
        IsSuperAdminOrAdmin(CurrentUser.Role);
        return await _settingService.UpdateSignInOptionAsync(signInOption).ConfigureAwait(false);
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
        await ValidateModelAsync(_initialSetupValidator, model).ConfigureAwait(false);
        var savedEntity = await generalSettingService
            .InitialSetupAsync(model)
            .ConfigureAwait(false);

        return new GeneralSettingResponseModel(savedEntity);
    }

    /// <summary>
    /// Validates the model using the provided validator.
    /// </summary>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <param name="validator"></param>
    /// <param name="model">The model to validate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task ValidateModelAsync<T>(IValidator<T> validator, T model)
    {
        await validator
            .ValidateAsync(model, options => options.ThrowOnFailures())
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Get the app version
    /// </summary>
    /// <returns>the app version</returns>
    private static string GetAppVersion()
    {
        return Assembly
            .GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
    }
}
