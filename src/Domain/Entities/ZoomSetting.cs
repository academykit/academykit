namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class ZoomSetting : AuditableEntity
    {
        public string SdkKey { get; set; }
        public string SdkSecret { get; set; }
        public string WebHookSecret { get; set; }
        public string OAuthAccountId { get; set; }
        public string OAuthClientId { get; set; }
        public string OAuthClientSecret { get; set; }
        public bool IsRecordingEnabled { get; set; }
        public User User { get; set; }
    }
}
