using AcademyKit.Application.Common.Dtos;
using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Interfaces;

public interface ISettingService
{
    /// <summary>
    /// Retrieves the allowed domains from the database as a single string.
    /// </summary>
    /// <returns>The allowed domains as a single string, or an empty string if not found.</returns>
    Task<string> GetAllowedDomainsAsync();

    /// <summary>
    /// Creates or updates the list of allowed domains in the database.
    /// </summary>
    /// <param name="domains">The list of domain strings to be allowed, separated by commas.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the updated list of allowed domains as a single string.</returns>
    Task<string> SetAllowedDomainsAsync(List<string> domains);

    /// <summary>
    /// Retrieves the default user role from the database.
    /// </summary>
    /// <returns>The default user role, or Trainer if not set.</returns>
    Task<UserRole> GetDefaultRole();

    /// <summary>
    /// Sets the default user role in the database.
    /// </summary>
    /// <param name="role">The user role to set as default.</param>
    /// <returns>The set default user role.</returns>
    Task<UserRole> SetDefaultRole(UserRole role);

    /// <summary>
    /// Retrieves all sign-in options from the database.
    /// </summary>
    /// <returns>A list of sign-in options.</returns>
    Task<List<SignInOptionDto>> GetSignInOptionsAsync();

    /// <summary>
    /// Updates a sign-in option in the database.
    /// </summary>
    /// <param name="signInOption">The sign-in option to update.</param>
    /// <returns>The updated sign-in option.</returns>
    Task<SignInOptionDto> UpdateSignInOptionAsync(SignInOptionDto signInOption);
}
