namespace AcademyKit.Infrastructure.Configurations;

/// <summary>
/// Configuration settings for JWT (JSON Web Token) authentication.
/// </summary>
public class JWT
{
    /// <summary>
    /// Gets or sets the secret key used for signing the JWT.
    /// </summary>
    /// <value>
    /// A secret key used to sign and validate the JWT.
    /// </value>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the issuer of the JWT.
    /// </summary>
    /// <value>
    /// The entity that issues the JWT, typically the application or service.
    /// </value>
    public string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the audience of the JWT.
    /// </summary>
    /// <value>
    /// The intended recipient of the JWT, typically the application or service.
    /// </value>
    public string Audience { get; set; }

    /// <summary>
    /// Gets or sets the duration for which the JWT is valid, in minutes.
    /// </summary>
    /// <value>
    /// The duration, in minutes, that the JWT remains valid.
    /// </value>
    public double DurationInMinutes { get; set; }
}
