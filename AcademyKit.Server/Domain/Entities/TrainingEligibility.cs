namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class TrainingEligibility : AuditableEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid? EligibilityId { get; set; }
        public virtual EligibilityCreation EligibilityCreation { get; set; }
        public TrainingEligibilityEnum TrainingEligibilityEnum { get; set; }
    }
}
