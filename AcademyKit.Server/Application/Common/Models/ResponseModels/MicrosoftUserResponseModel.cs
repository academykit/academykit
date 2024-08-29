namespace AcademyKit.Server.Application.Common.Models.ResponseModels;

/// <summary>
/// Represents the details of a Microsoft user.
/// </summary>
public class MicrosoftUserResponseModel
{
    /// <summary>
    /// Gets or sets the given name (first name) of the user.
    /// </summary>
    public string GivenName { get; set; }

    /// <summary>
    /// Gets or sets the surname (last name) of the user.
    /// </summary>
    public string Surname { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Mail { get; set; }

    /// <summary>
    /// Gets or sets the mobile phone number of the user.
    /// </summary>
    public string MobilePhone { get; set; }
}
