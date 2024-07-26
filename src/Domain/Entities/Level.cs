namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class Level : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
        public IList<Course> Courses { get; set; }
    }
}
