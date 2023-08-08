
using Microsoft.AspNetCore.Http;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public class GeneralSettingRequestModel
    {
        public string LogoUrl { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContactNumber { get; set; }
        public string EmailSignature { get; set; }
        public string CustomConfiguration { get; set; }
    }
}