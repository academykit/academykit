namespace AcademyKit.Infrastructure.Configurations;

/// <summary>
/// Base class for OAuth configuration settings.
/// </summary>
public abstract class OAuthConfiguration
{
    /// <summary>
    /// Gets or sets the Client ID for OAuth.
    /// </summary>
    /// <value>
    /// The unique identifier for the OAuth application.
    /// </value>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the Client Secret for OAuth.
    /// </summary>
    /// <value>
    /// The secret key used to authenticate the OAuth application.
    /// </value>
    public string ClientSecret { get; set; }
}
