namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionOption : AuditableEntity
    {
        public string Option { get; set; }
        public int Order { get; set; }
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
        public bool IsCorrect { get; set; }
        public User User { get; set; }
    }
}
