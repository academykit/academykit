namespace Lingtren.Application.Common.Models.RequestModels
{
    public class ZoomSettingRequestModel
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string SdkKey { get; set; }
        public string SdkSecret { get; set; }
        public bool IsRecordingEnabled { get; set; }
    }
}
