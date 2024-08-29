namespace AcademyKit.Server.Application.Common.Models.ResponseModels;

/// <summary>
/// Represents a common user response model for both Google and Microsoft.
/// </summary>
public class OAuthUserResponseModel
{
    /// <summary>
    /// Gets or sets the user's given name (first name).
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's family name (last name).
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's mobile phone number, if available.
    /// </summary>
    public string MobilePhone { get; set; }

    /// <summary>
    /// Gets or sets the URL of the user's profile picture, if available.
    /// </summary>
    public string ProfilePictureUrl { get; set; }
}
