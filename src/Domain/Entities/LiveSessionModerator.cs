namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class LiveSessionModerator : AuditableEntity
    {
        public Guid LiveSessionId { get; set; }
        public LiveSession LiveSession { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}