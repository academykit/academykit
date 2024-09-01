using Newtonsoft.Json;

namespace AcademyKit.Server.Application.Common.Models.ResponseModels;

/// <summary>
/// Represents the user information returned by the Google API.
/// </summary>
public class GoogleUserResponseModel
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's given name (first name).
    /// </summary>
    [JsonProperty("given_name")]
    public string GivenName { get; set; }

    /// <summary>
    /// Gets or sets the user's family name (last name).
    /// </summary>
    [JsonProperty("family_name")]
    public string FamilyName { get; set; }

    /// <summary>
    /// Gets or sets the URL of the user's profile picture.
    /// </summary>
    public string Picture { get; set; }
}
