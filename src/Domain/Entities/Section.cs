namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class Section : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public int Order { get; set; }
        public int Duration { get; set; }
        public bool IsDeleted { get; set; }
        public CourseStatus Status { get; set; }
        public IList<Lesson> Lessons { get; set; }
        public User User { get; set; }
    }
}
