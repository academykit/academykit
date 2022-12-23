namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class CourseCertificateResponseModel
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
}
