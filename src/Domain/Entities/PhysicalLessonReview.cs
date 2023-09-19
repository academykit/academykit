using Lingtren.Domain.Common;

namespace Lingtren.Domain.Entities
{
    public class PhysicalLessonReview : AuditableEntity
    {
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public string ReviewMessage { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool HasAttended { get; set; }
        public bool IsReviewed { get; set; }
    }
}
