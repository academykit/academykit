namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssessmentSubmissionAnswer : AuditableEntity
    {
        public Guid AssessmentSubmissionId { get; set; }
        public AssessmentSubmission AssessmentSubmission { get; set; }
        public Guid AssessmentQuestionId { get; set; }
        public AssessmentQuestion AssessmentQuestion { get; set; }
        public string SelectedAnswers { get; set; }
        public bool IsCorrect { get; set; }
        public User User { get; set; }
    }
}
