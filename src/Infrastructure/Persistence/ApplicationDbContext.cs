namespace Lingtren.Infrastructure.Persistence
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var userId = new Guid("30fcd978-f256-4733-840f-759181bc5e63");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Entity<User>().HasData(
                new User
                {
                    Id = userId,
                    FirstName = "ABC",
                    MiddleName = null,
                    LastName = "XYZ",
                    Address = "ADDRESS",
                    Email = "vuriloapp@gmail.com",
                    MobileNumber = "1234567890",
                    CreatedBy = userId,
                    CreatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    UpdatedBy = userId,
                    UpdatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    Status = UserStatus.Active,
                    HashPassword = "+gURQgHBT1zJz5AljZhAMyaNRFQBVorq5HIlEmhf+ZQ=:BBLvXedGXzdz0ZlypoKQxQ==",  // Admin@123
                    Role = UserRole.SuperAdmin,
                }
            );
            builder.Entity<SMTPSetting>().HasData(
                new SMTPSetting
                {
                    Id = new Guid("d3c343d8-adf8-45d4-afbe-e09c3285da24"),
                    MailPort = 123,
                    MailServer = "email-smtp.ap-south-1.amazonaws.com",
                    Password = "password",
                    ReplyTo = "support@vurilo.com",
                    SenderEmail = "noreply@vurilo.com",
                    SenderName = "Vurilo",
                    UserName = "username",
                    UseSSL = true,
                    CreatedBy = userId,
                    CreatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    UpdatedBy = userId,
                    UpdatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                }
            );
            builder.Entity<ZoomSetting>().HasData(
                new ZoomSetting
                {
                    Id = new Guid("f41a902f-fabd-4749-ac28-91137f685cb8"),
                    SdkKey = "sdk key value",
                    SdkSecret = "sdk secret value",
                    OAuthAccountId="OAuth account id",
                    OAuthClientId="OAuth client id",
                    OAuthClientSecret = "OAuth client secret",
                    IsRecordingEnabled = false,
                    CreatedBy = userId,
                    CreatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    UpdatedBy = userId,
                    UpdatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                }
            );
            builder.Entity<GeneralSetting>().HasData(
                new GeneralSetting
                {
                    Id = new Guid("2d7867fc-b7e7-461d-9257-d0990b5ac991"),
                    CompanyName = "company name",
                    CompanyAddress = "company address",
                    CompanyContactNumber = "company contact number",
                    EmailSignature = "company default email signature",
                    LogoUrl = "image path",
                    CreatedBy = userId,
                    CreatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    UpdatedBy = userId,
                    UpdatedOn = new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                }
            );
            builder.Entity<Setting>().HasData(
                new Setting
                {
                    Key = "Storage",
                    Value = nameof(StorageType.AWS)
                },
                new Setting
                {
                    Key = "AWS_AccessKey",
                },
                new Setting
                {
                    Key = "AWS_SecretKey",
                },
                new Setting
                {
                    Key = "AWS_FileBucket",
                },
                new Setting
                {
                    Key = "AWS_VideoBucket",
                },
                new Setting
                {
                    Key = "AWS_CloudFront",
                },
                new Setting
                {
                    Key = "AWS_RegionEndpoint"
                },
                new Setting
                {
                    Key = "Server_Url",
                },
                new Setting
                {
                    Key = "Server_Bucket",
                },
                new Setting
                {
                    Key = "Server_AccessKey",
                },
                new Setting
                {
                    Key = "Server_SecretKey",
                },
                 new Setting
                 {
                     Key = "Server_PresignedExpiryTime",
                 },
                new Setting
                {
                    Key = "Server_EndPoint",
                },
                new Setting
                {
                    Key = "Server_PresignedUrl",
                }
            );
            base.OnModelCreating(builder);
        }
    }
}
