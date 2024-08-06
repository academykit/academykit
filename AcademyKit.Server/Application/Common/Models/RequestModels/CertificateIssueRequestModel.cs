namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class CertificateIssueRequestModel
    {
        public IList<Guid> UserIds { get; set; }
        public bool IssueAll { get; set; }
    }
}
