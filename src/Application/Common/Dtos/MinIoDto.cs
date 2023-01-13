namespace Lingtren.Application.Common.Dtos
{
    public class MinIoDto
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Url { get; set; }
        public string Bucket { get; set; }
        public string EndPoint { get; set; }
        public int ExpiryTime { get; set; }
    }
}