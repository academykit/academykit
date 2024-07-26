namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class SkillsCriteria : AuditableEntity
    {
        public SkillAssessmentRule SkillAssessmentRule { get; set; }
        public decimal Percentage { get; set; }
        public Guid? SkillId { get; set; }
        public Skills SkillType { get; set; }
        public Assessment Assessment { get; set; }
        public Guid? AssessmentId { get; set; }
    }
}
