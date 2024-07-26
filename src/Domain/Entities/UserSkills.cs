namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class UserSkills : AuditableEntity
    {
        public Guid SkillId { get; set; }
        public Guid UserId { get; set; }
        public Skills Skills { get; set; }
        public User User { get; set; }
    }
}
