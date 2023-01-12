namespace Lingtren.Application.Common.Dtos
{
    public class AmazonSettingDto
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string RegionEndpoint { get; set; }
        public string FileBucket { get; set; }
        public string VideoBucket { get; set; }
        public string CloudFront { get; set; }
    }
}