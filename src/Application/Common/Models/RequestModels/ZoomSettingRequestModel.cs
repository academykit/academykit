namespace Lingtren.Application.Common.Models.RequestModels
{
    public class ZoomSettingRequestModel
    {
        public string OAuthAccountId { get; set; }
        public string OAuthClientId { get; set; }
        public string OAuthClientSecret { get; set; }
        public string SdkKey { get; set; }
        public string SdkSecret { get; set; }
        public string WebhookSecret { get; set; }
        public bool IsRecordingEnabled { get; set; }
    }
}
