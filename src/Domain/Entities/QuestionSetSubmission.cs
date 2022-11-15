namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionSetSubmission : AuditableEntity
    {
        public Guid QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public IList<QuestionSetResult> QuestionSetResults { get; set; }
        public IList<QuestionSetSubmissionAnswer> QuestionSetSubmissionAnswers { get; set; }
    }
}
