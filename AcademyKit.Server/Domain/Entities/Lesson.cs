namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class Lesson : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ExternalUrl { get; set; }
        public string DocumentUrl { get; set; }
        public int Order { get; set; }
        public int Duration { get; set; }
        public bool IsMandatory { get; set; }
        public LessonType Type { get; set; }
        public bool IsDeleted { get; set; }
        public CourseStatus Status { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid SectionId { get; set; }
        public Section Section { get; set; }
        public Guid? MeetingId { get; set; }
        public Meeting Meeting { get; set; }
        public Guid? QuestionSetId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public QuestionSet QuestionSet { get; set; }
        public User User { get; set; }
        public string VideoKey { get; set; }
        public IList<WatchHistory> WatchHistories { get; set; }
        public IList<CourseEnrollment> CourseEnrollments { get; set; }
        public IList<Assignment> Assignments { get; set; }
        public IList<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public IList<AssignmentReview> AssignmentReviews { get; set; }
        public IList<PhysicalLessonReview> physicalLessonReviews { get; set; }
    }
}
