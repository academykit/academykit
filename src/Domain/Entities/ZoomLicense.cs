namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class ZoomLicense : AuditableEntity
    {
        public string LicenseEmail { get; set; }
        public string HostId { get; set; }
        public User User { get; set; }
    }
}