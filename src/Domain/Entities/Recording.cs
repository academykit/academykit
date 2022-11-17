namespace Lingtren.Domain.Entities
{
    public class Recording
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}