namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class WatchHistory : AuditableEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool IsCompleted { get; set; }
    }
}
