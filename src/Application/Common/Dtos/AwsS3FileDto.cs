using Microsoft.AspNetCore.Http;

namespace AcademyKit.Application.Common.Dtos
{
    public class AwsS3FileDto
    {
        public string Key { get; set; }
        public AmazonSettingDto Setting { get; set; }
        public IFormFile File { get; set; }
        public MediaType Type { get; set; }
        public string FilePath { get; set; }
    }
}
