namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class LiveSessionTag : AuditableEntity
    {
        public Guid LiveSessionId { get; set; }
        public LiveSession LiveSession { get; set; }
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
        public User User { get; set; }
    }
}