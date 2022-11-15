namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionSetSubmissionAnswer : AuditableEntity
    {
        public Guid QuestionSetSubmissionId { get; set; }
        public QuestionSetSubmission QuestionSetSubmission { get; set; }
        public Guid QuestionSetQuestionId { get; set; }
        public QuestionSetQuestion QuestionSetQuestion { get; set; }
        public string SelectedAnswers { get; set; }
        public bool IsCorrect { get; set; }
        public User User { get; set; }
    }
}
