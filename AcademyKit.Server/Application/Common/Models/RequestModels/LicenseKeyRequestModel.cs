using System.ComponentModel.DataAnnotations;

namespace AcademyKit.Application.Common.Models.RequestModels;

/// <summary>
/// Represents a request model for license key validation.
/// </summary>
public class LicenseKeyRequestModel
{
    /// <summary>
    /// Gets or sets the license key to be validated.
    /// </summary>
    /// <remarks>
    /// This property is required and must be provided when submitting the request.
    /// </remarks>
    [Required]
    public string LicenseKey { get; set; }
}
