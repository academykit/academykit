namespace AcademyKit.Server.Application.Common.Interfaces;

public interface ISettingService
{
    /// <summary>
    /// Retrieves the allowed domains from the database as a single string.
    /// </summary>
    /// <returns>The allowed domains as a single string, or an empty string if not found.</returns>
    Task<string> GetAllowedDomainsAsync();

    /// <summary>
    /// Updates the list of allowed domains in the database.
    /// </summary>
    /// <param name="domains">The list of domain strings to be allowed, separated by commas.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the updated list of allowed domains as a single string.</returns>
    Task<string> UpdateAllowedDomainsAsync(string domains);
}
