namespace Lingtren.Infrastructure.Persistence
{
    using System.Reflection;
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<CourseTeacher> CourseTeachers { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }
        public DbSet<ZoomSetting> ZoomSettings { get; set; }
        public DbSet<ZoomLicense> ZoomLicenses { get; set; }
        public DbSet<SMTPSetting> SMTPSettings { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingReport> MeetingReports { get; set; }
        public DbSet<GeneralSetting> GeneralSettings { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<QuestionSet> QuestionSets { get; set; }
        public DbSet<QuestionPool> QuestionPools { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<QuestionPoolQuestion> QuestionPoolQuestions { get; set; }
        public DbSet<QuestionSetQuestion> QuestionSetQuestions { get; set; }
        public DbSet<QuestionPoolTeacher> QuestionPoolTeachers { get; set; }
        public DbSet<QuestionTag> QuestionTags { get; set; }
        public DbSet<QuestionSetSubmission> QuestionSetSubmissions { get; set; }
        public DbSet<QuestionSetSubmissionAnswer> QuestionSetSubmissionAnswers { get; set; }
        public DbSet<QuestionSetResult> QuestionSetResults { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentAttachment> AssignmentAttachments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<AssignmentQuestionOption> AssignmentQuestionOptions { get; set; }
        public DbSet<AssignmentSubmissionAttachment> AssignmentSubmissionAttachments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReply> CommentReplies { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<GroupFile> GroupFiles { get; set; }
        public DbSet<AssignmentReview> AssignmentReviews { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackQuestionOption> FeedbackQuestionOptions { get; set; }
        public DbSet<FeedbackSubmission> FeedbackSubmissions { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<PhysicalLessonReview> PhysicalLessonReviews { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<SkillsCriteria> SkillsCriteria { get; set; }
        public DbSet<UserSkills> UserSkills { get; set; }
        public DbSet<EligibilityCreation> EligibilityCreations { get; set; }
        public DbSet<AIKey> AIKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
}
