namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class GeneralSettingResponseModel
    {
        public Guid Id { get; set; }
        public string LogoUrl { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContactNumber { get; set; }
        public string EmailSignature { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}