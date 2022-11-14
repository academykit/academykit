namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class QuestionPoolTeacher : AuditableEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid QuestionPoolId { get; set; }
        public QuestionPool QuestionPool { get; set; }
        public PoolRole Role { get; set; }
    }
}
