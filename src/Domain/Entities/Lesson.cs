namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;
    public class Lesson : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Order { get; set; }
        public int Duration { get; set; }
        public bool IsPreview { get; set; }
        public bool IsDeleted { get; set; }
        public Status Status { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid SectionId { get; set; }
        public Section Section { get; set; }
        public User User { get; set; }
    }
}