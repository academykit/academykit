namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssignmentReview : AuditableEntity
    {
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public decimal Mark { get; set; }
        public string Review { get; set; }
        public bool IsDeleted { get; set; }
    }
}
