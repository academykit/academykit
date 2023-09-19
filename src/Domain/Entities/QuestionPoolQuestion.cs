namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionPoolQuestion : AuditableEntity
    {
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
        public Guid QuestionPoolId { get; set; }
        public QuestionPool QuestionPool { get; set; }
        public User User { get; set; }
        public IList<QuestionSetQuestion> QuestionSetQuestions { get; set; }
        public int Order { get; set; }
    }
}
