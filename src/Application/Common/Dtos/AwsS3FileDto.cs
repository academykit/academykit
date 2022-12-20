using Microsoft.AspNetCore.Http;

namespace Lingtren.Application.Common.Dtos
{
    public class AwsS3FileDto
    {
        public string Key { get; set; }
        public AmazonSettingModel Setting { get; set; }
        public IFormFile File { get; set; }
    }
}