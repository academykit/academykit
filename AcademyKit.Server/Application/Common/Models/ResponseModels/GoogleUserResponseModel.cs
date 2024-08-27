namespace AcademyKit.Server.Application.Common.Models.ResponseModels;

/// <summary>
/// Represents the user information returned by the Google API.
/// </summary>
public class GoogleUserResponseModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user's email is verified.
    /// </summary>
    public bool VerifiedEmail { get; set; }

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the user's given name (first name).
    /// </summary>
    public string GivenName { get; set; }

    /// <summary>
    /// Gets or sets the user's family name (last name).
    /// </summary>
    public string FamilyName { get; set; }

    /// <summary>
    /// Gets or sets the URL of the user's profile picture.
    /// </summary>
    public string Picture { get; set; }

    /// <summary>
    /// Gets or sets the user's locale (language/region).
    /// </summary>
    public string Locale { get; set; }
}
