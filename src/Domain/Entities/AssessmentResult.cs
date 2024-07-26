namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class AssessmentResult : AuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid AssessmentId { get; set; }
        public Assessment Assessment { get; set; }
        public Guid AssessmentSubmissionId { get; set; }
        public AssessmentSubmission AssessmentSubmission { get; set; }
        public decimal TotalMark { get; set; }
        public decimal NegativeMark { get; set; }
        public User User { get; set; }
    }
}
