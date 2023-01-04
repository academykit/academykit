namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class CourseEnrollment : AuditableEntity
    {
        public Guid? CurrentLessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int CurrentLessonWatched { get; set; }
        public int Percentage { get; set; }
        public EnrollmentMemberStatusEnum EnrollmentMemberStatus { get; set; }
        public string ActivityReason { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? HasCertificateIssued { get; set; }
        public string CertificateUrl { get; set; }
        public DateTime? CertificateIssuedDate { get; set; }
    }
}
