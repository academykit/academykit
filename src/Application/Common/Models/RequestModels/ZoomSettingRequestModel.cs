namespace Lingtren.Application.Common.Models.RequestModels
{
    public class ZoomSettingRequestModel
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public bool IsRecordingEnabled { get; set; }
    }
}
