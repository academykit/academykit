namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionSetQuestion : AuditableEntity
    {
        public Guid QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public Guid? QuestionId { get; set; }
        public Question Question { get; set; }
        public int Order { get; set; }
        public Guid? QuestionPoolQuestionId { get; set; }
        public QuestionPoolQuestion QuestionPoolQuestion { get; set; }
        public User User { get; set; }
    }
}