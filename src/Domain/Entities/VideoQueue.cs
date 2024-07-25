namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Enums;

    public class VideoQueue
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public string VideoUrl { get; set; }
        public VideoStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
