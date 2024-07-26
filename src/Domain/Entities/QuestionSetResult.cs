namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class QuestionSetResult : AuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public Guid QuestionSetSubmissionId { get; set; }
        public QuestionSetSubmission QuestionSetSubmission { get; set; }
        public decimal TotalMark { get; set; }
        public decimal NegativeMark { get; set; }
        public User User { get; set; }
    }
}
