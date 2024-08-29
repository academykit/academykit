namespace AcademyKit.Server.Application.Common.Interfaces;

/// <summary>
/// Provides methods for hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes the provided password using the optional salt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">An optional salt to use for hashing. If not provided, a default salt is used.</param>
    /// <param name="needsOnlyHash">Specifies whether to return only the hashed password without the salt.</param>
    /// <returns>The hashed password, or the hashed password with salt if <paramref name="needsOnlyHash"/> is <c>false</c>.</returns>
    string HashPassword(string password, byte[] salt = null, bool needsOnlyHash = false);

    /// <summary>
    /// Verifies the provided password against the hashed password.
    /// </summary>
    /// <param name="hashedPasswordWithSalt">The hashed password (including salt) to compare against.</param>
    /// <param name="password">The plain text password to verify.</param>
    /// <returns><c>true</c> if the password is correct; otherwise, <c>false</c>.</returns>
    bool VerifyPassword(string hashedPasswordWithSalt, string password);
}
