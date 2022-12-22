namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;
    using System.Text.RegularExpressions;

    public class User : AuditableEntity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public UserRole Role { get; set; }
        public string Profession { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public string HashPassword { get; set; }
        public string PublicUrls { get; set; }
        public bool IsActive { get; set; }
        public string PasswordResetToken { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string PasswordChangeToken { get; set; }
        public Guid? DepartmentId { get; set; }
        public Department Department { get; set; }
        public IList<RefreshToken> RefreshTokens { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<GroupMember> GroupMembers { get; set; }
        public IList<Tag> Tags { get; set; }
        public IList<Level> Levels { get; set; }
        public IList<Course> Courses { get; set; }
        public IList<Section> Sections { get; set; }
        public IList<Lesson> Lessons { get; set; }
        public IList<CourseTeacher> CourseTeachers { get; set; }
        public IList<ZoomSetting> ZoomSettings { get; set; }
        public IList<ZoomLicense> ZoomLicenses { get; set; }
        public IList<SMTPSetting> SMTPSettings { get; set; }
        public IList<Meeting> Meetings { get; set; }
        public IList<MeetingReport> MeetingReports { get; set; }
        public IList<GeneralSetting> GeneralSettings { get; set; }
        public IList<Department> Departments { get; set; }
        public IList<CourseTag> CourseTags { get; set; }
        public IList<QuestionSet> QuestionSets { get; set; }
        public IList<QuestionPool> QuestionPools { get; set; }
        public IList<Question> Questions { get; set; }
        public IList<QuestionOption> QuestionOptions { get; set; }
        public IList<QuestionSetQuestion> QuestionSetQuestions { get; set; }
        public IList<QuestionPoolQuestion> QuestionPoolQuestions { get; set; }
        public IList<QuestionPoolTeacher> QuestionPoolTeachers { get; set; }
        public IList<QuestionSetSubmission> QuestionSetSubmissions { get; set; }
        public IList<QuestionSetSubmissionAnswer> QuestionSetSubmissionAnswers { get; set; }
        public IList<QuestionSetResult> QuestionSetResults { get; set; }
        public IList<CourseEnrollment> CourseEnrollments { get; set; }
        public IList<WatchHistory> WatchHistories { get; set; }
        public IList<Assignment> Assignments { get; set; }
        public IList<AssignmentAttachment> AssignmentAttachments { get; set; }
        public IList<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public IList<QuestionTag> QuestionTags { get; set; }
        public IList<AssignmentQuestionOption> AssignmentQuestionOptions { get; set; }
        public IList<AssignmentSubmissionAttachment> AssignmentSubmissionAttachments { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<CommentReply> CommentReplies { get; set; }
        public IList<GroupFile> GroupStorages { get; set; }
        public IList<AssignmentReview> AssignmentReviews { get; set; }
        public IList<Feedback> Feedbacks { get; set; }
        public IList<FeedbackQuestionOption> FeedbackQuestionOptions { get; set; }
        public IList<FeedbackSubmission> FeedbackSubmissions { get; set; }


        /// <summary>
        /// Get or set full name
        /// </summary>
        public string FullName
        {
            get
            {
                return Regex.Replace($"{FirstName} {MiddleName} {LastName}", @"\s+", " ");
            }
        }
    }
}
