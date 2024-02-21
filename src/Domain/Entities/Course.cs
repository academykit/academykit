namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class Course : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? GroupId { get; set; }
        public Group Group { get; set; }
        public CourseStatus Status { get; set; }
        public Language Language { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Duration { get; set; }
        public bool IsUpdate { get; set; }
        public Guid LevelId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsUnlimitedEndDate { get; set; }
        public Level Level { get; set; }
        public User User { get; set; }
        public IList<Section> Sections { get; set; }
        public IList<Lesson> Lessons { get; set; }
        public IList<CourseTeacher> CourseTeachers { get; set; }
        public IList<CourseTag> CourseTags { get; set; }
        public IList<WatchHistory> WatchHistories { get; set; }
        public IList<CourseEnrollment> CourseEnrollments { get; set; }
        public IList<Signature> Signatures { get; set; }
        public IList<EligibilityCreation> EligibilityCreations { get; set; }

        public CourseCertificate CourseCertificate { get; set; }
        public IList<TrainingEligibility> TrainingEligibilities { get; set; }
    }
}
