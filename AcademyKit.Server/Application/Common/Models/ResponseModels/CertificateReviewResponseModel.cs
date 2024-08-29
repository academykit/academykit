namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class CertificateReviewResponseModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string CertificateName { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
