namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class AssessmentSubmission : AuditableEntity
    {
        public Guid AssessmentId { get; set; }
        public Assessment Assessment { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSubmissionError { get; set; }
        public string SubmissionErrorMessage { get; set; }
        public IList<AssessmentSubmissionAnswer> AssessmentSubmissionAnswers { get; set; }
        public IList<AssessmentResult> AssessmentResults { get; set; }
    }
}
