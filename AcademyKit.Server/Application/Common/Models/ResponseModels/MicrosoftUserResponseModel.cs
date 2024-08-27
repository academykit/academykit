namespace AcademyKit.Server.Application.Common.Models.ResponseModels;

/// <summary>
/// Represents the details of a Microsoft user.
/// </summary>
public class MicrosoftUserResponseModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the given name (first name) of the user.
    /// </summary>
    public string GivenName { get; set; }

    /// <summary>
    /// Gets or sets the surname (last name) of the user.
    /// </summary>
    public string Surname { get; set; }

    /// <summary>
    /// Gets or sets the user principal name (UPN) of the user, typically an email address.
    /// </summary>
    public string UserPrincipalName { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Mail { get; set; }

    /// <summary>
    /// Gets or sets the job title of the user.
    /// </summary>
    public string JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the mobile phone number of the user.
    /// </summary>
    public string MobilePhone { get; set; }

    /// <summary>
    /// Gets or sets the office location of the user.
    /// </summary>
    public string OfficeLocation { get; set; }

    /// <summary>
    /// Gets or sets the preferred language of the user.
    /// </summary>
    public string PreferredLanguage { get; set; }

    /// <summary>
    /// Gets or sets the business phone numbers of the user.
    /// </summary>
    public string[] BusinessPhones { get; set; }
}
