using Newtonsoft.Json;

namespace AcademyKit.Application.Common.Models.ResponseModels;

/// <summary>
/// Represents the response received from an OAuth token request.
/// </summary>
public class OAuthTokenResponse
{
    /// <summary>
    /// Gets or sets the access token issued by the authorization server.
    /// </summary>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the lifetime of the access token in seconds.
    /// </summary>
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the refresh token which can be used to obtain new access tokens.
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the scope of the access token.
    /// </summary>
    public string Scope { get; set; }

    /// <summary>
    /// Gets or sets the type of the token issued (e.g., Bearer).
    /// </summary>
    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the ID token, which contains identity information about the user.
    /// </summary>
    [JsonProperty("id_token")]
    public string IdToken { get; set; }

    /// <summary>
    /// Gets or sets the error code returned if the request was unsuccessful.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets a description of the error returned if the request was unsuccessful.
    /// </summary>
    [JsonProperty("error_description")]
    public string ErrorDescription { get; set; }

    /// <summary>
    /// Gets a value indicating whether the token request was successful.
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(Error);
}
