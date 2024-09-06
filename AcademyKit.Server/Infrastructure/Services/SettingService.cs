using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Localization;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Infrastructure.Services;

/// <summary>
/// Provides services for managing application settings, including retrieving and updating allowed domains, default roles, and sign-in options.
/// </summary>
public class SettingService : BaseService, ISettingService
{
    private const string SignInPrefix = "SignIn_";
    private const string AllowedDomainsKey = "AllowedDomains";
    private const string DefaultRoleKey = "DefaultRole";

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used for database operations.</param>
    /// <param name="logger">The logger used for logging operations.</param>
    /// <param name="localizer">The localizer used for handling localized strings.</param>
    public SettingService(
        IUnitOfWork unitOfWork,
        ILogger<SettingService> logger,
        IStringLocalizer<ExceptionLocalizer> localizer
    )
        : base(unitOfWork, logger, localizer) { }

    /// <summary>
    /// Retrieves the allowed domains from the database as a single string.
    /// </summary>
    /// <returns>The allowed domains as a single string, or an empty string if not found.</returns>
    public async Task<string> GetAllowedDomainsAsync()
    {
        return await ExecuteWithResultAsync(async () =>
            {
                var setting = await GetSettingAsync(AllowedDomainsKey).ConfigureAwait(false);
                return setting?.Value ?? string.Empty;
            })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates or updates the list of allowed domains in the database.
    /// </summary>
    /// <param name="domains">The list of domain strings to be allowed, separated by commas.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the updated list of allowed domains as a single string.</returns>
    public async Task<string> SetAllowedDomainsAsync(string domains)
    {
        return await ExecuteWithResultAsync(async () =>
            {
                var formattedDomains = FormatDomains(domains);
                var setting = await GetOrCreateSettingAsync(AllowedDomainsKey, formattedDomains)
                    .ConfigureAwait(false);
                return setting.Value;
            })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the default user role from the database.
    /// </summary>
    /// <returns>The default user role, or Trainer if not set.</returns>
    public async Task<UserRole> GetDefaultRole()
    {
        return await ExecuteWithResultAsync(async () =>
            {
                var setting = await GetSettingAsync(DefaultRoleKey).ConfigureAwait(false);
                return setting is null ? UserRole.Trainer : Enum.Parse<UserRole>(setting.Value);
            })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the default user role in the database.
    /// </summary>
    /// <param name="role">The user role to set as default.</param>
    /// <returns>The set default user role.</returns>
    public async Task<UserRole> SetDefaultRole(UserRole role)
    {
        return await ExecuteWithResultAsync(async () =>
            {
                await GetOrCreateSettingAsync(DefaultRoleKey, role.ToString())
                    .ConfigureAwait(false);
                return role;
            })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all sign-in options from the database.
    /// </summary>
    /// <returns>A list of sign-in options.</returns>
    public async Task<List<SignInOptionDto>> GetSignInOptionsAsync()
    {
        return await ExecuteWithResultAsync(async () =>
            {
                var signInSettings = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetAllAsync(predicate: s => s.Key.StartsWith(SignInPrefix))
                    .ConfigureAwait(false);

                return signInSettings
                    .Select(s => new SignInOptionDto
                    {
                        SignIn = Enum.Parse<SignInType>(s.Key.Substring(SignInPrefix.Length)),
                        IsAllowed = bool.Parse(s.Value)
                    })
                    .ToList();
            })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a sign-in option in the database.
    /// </summary>
    /// <param name="signInOption">The sign-in option to update.</param>
    /// <returns>The updated sign-in option.</returns>
    public async Task<SignInOptionDto> UpdateSignInOptionAsync(SignInOptionDto signInOption)
    {
        return await ExecuteWithResultAsync(async () =>
            {
                var key = $"{SignInPrefix}{signInOption.SignIn}";
                await GetOrCreateSettingAsync(key, signInOption.IsAllowed.ToString().ToLower())
                    .ConfigureAwait(false);
                return signInOption;
            })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a setting from the database based on the provided key.
    /// </summary>
    /// <param name="key">The key of the setting to retrieve.</param>
    /// <returns>The setting if found, otherwise null.</returns>
    private async Task<Setting> GetSettingAsync(string key)
    {
        return await _unitOfWork
            .GetRepository<Setting>()
            .GetFirstOrDefaultAsync(predicate: x => x.Key.ToLower() == key.ToLower())
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new setting if it does not exist, otherwise updates the existing setting.
    /// </summary>
    /// <param name="key">The key of the setting to create or update.</param>
    /// <param name="value">The value of the setting to create or update.</param>
    /// <returns>The created or updated setting.</returns>
    private async Task<Setting> GetOrCreateSettingAsync(string key, string value)
    {
        var settingRepo = _unitOfWork.GetRepository<Setting>();
        var setting = await GetSettingAsync(key).ConfigureAwait(false);

        if (setting == null)
        {
            setting = new Setting { Key = key, Value = value };
            await settingRepo.InsertAsync(setting).ConfigureAwait(false);
        }
        else
        {
            setting.Value = value;
            settingRepo.Update(setting);
        }

        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        return setting;
    }

    private static string FormatDomains(string domains)
    {
        return string.Join(",", domains.Split(',').Select(domain => domain.Trim()));
    }
}
