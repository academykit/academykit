namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class Skills : AuditableEntity
    {
        public string SkillName { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public IList<UserSkills> UserSkills { get; }
        public IList<User> Users { get; }
        public IList<EligibilityCreation> EligibilityCreations { get; set; }
        public IList<SkillsCriteria> SkillsCriteria { get; set; }
    }
}
