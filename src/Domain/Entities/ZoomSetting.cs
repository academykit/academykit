namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class ZoomSetting : AuditableEntity
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public bool IsRecordingEnabled { get; set; }
        public User User { get; set; }
    }
}