namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class TrainingEligibility : AuditableEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid? EligibilityId { get; set; }
        public TrainingEligibilityEnum TrainingEligibilityEnum { get; set; }
    }
}
