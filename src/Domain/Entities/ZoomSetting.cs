namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class ZoomSetting : AuditableEntity
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string SdkKey { get; set; }
        public string SdkSecret { get; set; }
        public bool IsRecordingEnabled { get; set; }
        public User User { get; set; }
    }
}