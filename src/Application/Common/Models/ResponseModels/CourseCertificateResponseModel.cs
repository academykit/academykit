using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class CourseCertificateIssuedResponseModel
    {
        public Guid CourseId { get; set; }
        public string CourseSlug { get; set; }
        public string CourseName { get; set; }
        public UserModel User { get; set; }
        public int Percentage { get; set; }
        public bool? HasCertificateIssued { get; set; }
        public string CertificateUrl { get; set; }
        public DateTime? CertificateIssuedDate { get; set; }
    }

    public class CourseCertificateResponseModel
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string SampleUrl { get; set; }
        public CourseCertificateResponseModel(CourseCertificate model)
        {
            Id = model.Id;
            CourseId = model.CourseId;
            Title = model.Title;
            EventStartDate = model.EventStartDate;
            EventEndDate = model.EventEndDate;
            SampleUrl = model.SampleUrl;
        }
        public CourseCertificateResponseModel()
        {

        }
    }
}
