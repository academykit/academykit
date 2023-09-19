namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class ExternalCertificateResponseModel
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Institute { get; set; }
        public string Duration { get; set; }
    }
}
