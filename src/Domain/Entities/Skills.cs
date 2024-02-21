namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class Skills : AuditableEntity
    {
        public string SkillName { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public IList<UserSkills> UserSkills { get; }
        public IList<EligibilityCreation> EligibilityCreations { get; set; }
        public IList<SkillsCriteria> SkillsCriteria { get; set; }
    }
}
