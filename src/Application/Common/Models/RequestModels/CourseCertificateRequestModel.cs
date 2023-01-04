namespace Lingtren.Application.Common.Models.RequestModels
{
    public class CourseCertificateRequestModel
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
    }
}
