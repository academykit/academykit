namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class Tag : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
        public IList<CourseTag> CourseTags { get; set; }
    }
}
