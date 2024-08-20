namespace AcademyKit.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class LicenseRequestModel
    {
        [Required]
        public string LicenseKey { get; set; }
    }
}