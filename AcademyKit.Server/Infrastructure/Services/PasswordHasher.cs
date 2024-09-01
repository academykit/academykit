using System.Security.Cryptography;
using AcademyKit.Application.Common.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AcademyKit.Infrastructure.Services;

/// <summary>
/// Provides methods for hashing and verifying passwords using PBKDF2.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hashes the provided password using the optional salt.
    /// If the salt is not provided or is invalid, a new salt is generated.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">An optional salt to use for hashing. If <c>null</c>, a new salt will be generated.</param>
    /// <param name="needsOnlyHash">Specifies whether to return only the hashed password without the salt.</param>
    /// <returns>The hashed password. If <paramref name="needsOnlyHash"/> is <c>false</c>, the return value will be in the format "hashedPassword:salt".</returns>
    public string HashPassword(string password, byte[] salt = null, bool needsOnlyHash = false)
    {
        if (salt == null || salt.Length != 16)
        {
            // Generate a 128-bit salt using a secure PRNG
            salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
        }

        var hashed = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            )
        );

        if (needsOnlyHash)
        {
            return hashed;
        }

        // Concatenate the hashed password and salt using ':'
        return $"{hashed}:{Convert.ToBase64String(salt)}";
    }

    /// <summary>
    /// Verifies the provided password against the hashed password.
    /// </summary>
    /// <param name="hashedPasswordWithSalt">The hashed password (including salt) to compare against.</param>
    /// <param name="password">The plain text password to verify.</param>
    /// <returns><c>true</c> if the password is correct; otherwise, <c>false</c>.</returns>
    public bool VerifyPassword(string hashedPasswordWithSalt, string password)
    {
        // Retrieve both salt and password hash from 'hashedPasswordWithSalt'
        var passwordAndHash = hashedPasswordWithSalt.Split(':');
        if (passwordAndHash.Length != 2)
        {
            return false;
        }

        var salt = Convert.FromBase64String(passwordAndHash[1]);
        if (salt == null)
        {
            return false;
        }

        // Hash the provided password using the extracted salt
        var hashOfPasswordToCheck = HashPassword(password, salt, true);

        // Compare the provided hash with the computed hash
        return string.Compare(passwordAndHash[0], hashOfPasswordToCheck) == 0;
    }
}
