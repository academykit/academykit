namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class CourseTeacher : AuditableEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
