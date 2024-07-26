namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class QuestionSetSubmission : AuditableEntity
    {
        public Guid QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSubmissionError { get; set; }
        public string SubmissionErrorMessage { get; set; }
        public IList<QuestionSetResult> QuestionSetResults { get; set; }
        public IList<QuestionSetSubmissionAnswer> QuestionSetSubmissionAnswers { get; set; }
    }
}
