using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class CertificateResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Institute { get; set; }
        public string Duration { get; set; }
        public bool IsVerified { get; set; }
        public UserModel User { get; set; }

        public CertificateResponseModel()
        {

        }

        public CertificateResponseModel(Certificate certificate)
        {
            Id = certificate.Id;
            Slug = certificate.Slug;
            Name = certificate.Name;
            StartDate = certificate.StartDate;
            EndDate = certificate.EndDate;
            ImageUrl = certificate.ImageUrl;
            Location = certificate.Location;
            Institute = certificate.Institute;
            Duration = certificate.Duration != 0 ? certificate.Duration.ToString() : null;
            IsVerified = certificate.IsVerified;

        }
    }
}