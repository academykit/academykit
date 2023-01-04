namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class GroupMember : AuditableEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public bool IsActive { get; set; }
    }
}