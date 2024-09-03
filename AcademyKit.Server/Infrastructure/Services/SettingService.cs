using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Localization;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Infrastructure.Services;

/// <summary>
/// Provides services for managing application settings, including retrieving and updating allowed domains.
/// </summary>
public class SettingService : BaseService, ISettingService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used for database operations.</param>
    /// <param name="logger">The logger used for logging operations.</param>
    /// <param name="localizer">The localizer used for handling localized strings.</param>
    public SettingService(
        IUnitOfWork unitOfWork,
        ILogger<MediaService> logger,
        IStringLocalizer<ExceptionLocalizer> localizer
    )
        : base(unitOfWork, logger, localizer) { }

    /// <summary>
    /// Retrieves the allowed domains from the database as a single string.
    /// </summary>
    /// <returns>The allowed domains as a single string, or an empty string if not found.</returns>
    public async Task<string> GetAllowedDomainsAsync()
    {
        var allowedDomainsSetting = await _unitOfWork
            .GetRepository<Setting>()
            .GetFirstOrDefaultAsync(predicate: x => x.Key == "AllowedDomains")
            .ConfigureAwait(false);

        return allowedDomainsSetting?.Value ?? string.Empty;
    }

    /// <summary>
    /// Updates the list of allowed domains in the database.
    /// </summary>
    /// <param name="domains">The list of domain strings to be allowed, separated by commas.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the updated list of allowed domains as a single string.</returns>
    public async Task<string> UpdateAllowedDomainsAsync(string domains)
    {
        var allowedDomainsSetting = await _unitOfWork
            .GetRepository<Setting>()
            .GetFirstOrDefaultAsync(predicate: x => x.Key == "AllowedDomains")
            .ConfigureAwait(false);

        var settingRepo = _unitOfWork.GetRepository<Setting>();

        // Split, trim, and join domains with a comma
        domains = string.Join(",", domains.Split(',').Select(domain => domain.Trim()));

        if (allowedDomainsSetting == null)
        {
            allowedDomainsSetting = new Setting { Key = "AllowedDomains", Value = domains };
            await settingRepo.InsertAsync(allowedDomainsSetting).ConfigureAwait(false);
        }
        else
        {
            allowedDomainsSetting.Value = domains;
            settingRepo.Update(allowedDomainsSetting);
        }
        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        return allowedDomainsSetting.Value;
    }
}
