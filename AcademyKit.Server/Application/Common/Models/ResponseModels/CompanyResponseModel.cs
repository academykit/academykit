namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class CompanyResponseModel
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string CustomConfiguration { get; set; }
        public string AppVersion { get; set; }
        public bool? IsSetupCompleted { get; set; }
    }
}
