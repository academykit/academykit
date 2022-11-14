namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionTag : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
        public User User { get; set; }
    }
}
