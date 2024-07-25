namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class ZoomLicense : AuditableEntity
    {
        public string LicenseEmail { get; set; }
        public string HostId { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
        public IList<Meeting> Meetings { get; set; }
    }
}
