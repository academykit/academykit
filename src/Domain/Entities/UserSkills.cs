namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class UserSkills : AuditableEntity
    {
        public Guid SkillId { get; set; }
        public Guid UserId { get; set; }
        public Skills Skills { get; set; }
        public User User { get; set; }
    }
}
