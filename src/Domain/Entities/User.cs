namespace Lingtren.Domain.Entities
{
    using System.Text.RegularExpressions;
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class User : AuditableEntity
    {
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public string HashPassword { get; set; }
        public string PublicUrls { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string PasswordChangeToken { get; set; }
        #region Basic
        public string ImageUrl { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Gender? Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public Nationality? Nationality { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public DateTime? BirthDateBS { get; set; }
        public DateTime? BirthDateAD { get; set; }
        #endregion
        #region Official Info
        public string EmploymentType { get; set; }
        public Guid? BranchId { get; set; }
        public Branch Branch { get; set; }
        public string MemberId { get; set; }
        public string Profession { get; set; }
        public Guid? DepartmentId { get; set; }
        public Department Department { get; set; }
        public DateTime? JoinedDateBS { get; set; }
        public DateTime? JoinedDateAD { get; set; }
        #endregion
        #region Address
        #region Permanent Address
        public string PermanentCountry { get; set; }
        public string PermanentState { get; set; }
        public string PermanentDistrict { get; set; }
        public string PermanentCity { get; set; }
        public string PermanentMunicipality { get; set; }
        public string PermanentWard { get; set; }
        public string Address { get; set; }
        #endregion
        #region Current Address
        public bool AddressIsSame { get; set; }
        public string CurrentCountry { get; set; }
        public string CurrentState { get; set; }
        public string CurrentDistrict { get; set; }
        public string CurrentCity { get; set; }
        public string CurrentMunicipality { get; set; }
        public string CurrentWard { get; set; }
        public string CurrentAddress { get; set; }
        #endregion
        #endregion
        #region Contact Details
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string PersonalEmail { get; set; }
        public string MobileNumberSecondary { get; set; }
        #endregion
        #region Identification
        public IdentityType? IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public string IdentityIssuedBy { get; set; }
        public DateTime? IdentityIssuedOn { get; set; }
        #endregion
        #region Family Information
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SpouseName { get; set; }
        public string GrandFatherName { get; set; }
        public string MemberPhone { get; set; }
        public string MemberPermanentAddress { get; set; }
        public string MemberCurrentAddress { get; set; }
        public bool FamilyAddressIsSame { get; set; }
        #endregion
        public string Bio { get; set; }
        #region navigation properties
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
        public IList<Signature> Signatures { get; set; }
        public IList<CourseCertificate> CourseCertificates { get; set; }
        public IList<Certificate> Certificates { get; set; }
        public IList<UserEducation> UserEducations { get; set; }
        public IList<UserWorkExperience> WorkExperiences { get; set; }

        #endregion

        /// <summary>
        /// Get or set full name
        /// </summary>
        public string FullName => Regex.Replace($"{FirstName} {MiddleName} {LastName}", @"\s+", " ");
    }
}
