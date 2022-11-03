namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;
    public class LiveSessionMember : AuditableEntity
    {
        public Guid LiveSessionId { get; set; }
        public LiveSession LiveSession { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool IsDeleted { get; set; }
        public EnrollmentStatus Status { get; set; }
    }
}