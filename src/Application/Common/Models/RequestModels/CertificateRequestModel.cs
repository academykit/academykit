namespace Lingtren.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    public class CertificateRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Institute { get; set; }
        public int Duration { get; set; }
    }
}