namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class EligibilityCreation : AuditableEntity
    {
        public UserRole Role { get; set; }
        public Guid? SkillId { get; set; }
        public Skills Skills { get; set; }
        public Guid? TrainingId { get; set; }
        public Course Course { get; set; }
        public Guid? DepartmentId { get; set; }
        public Department Department { get; set; }
        public Guid? GroupId { get; set; }
        public Group Group { get; set; }
        public Guid? CompletedAssessmentId { get; set; }
        public Assessment CompletedAssessment { get; set; }
        public Assessment Assessment { get; set; }
        public Guid? AssessmentId { get; set; }
        public virtual TrainingEligibility TrainingEligibility { get; set; }
    }
}
