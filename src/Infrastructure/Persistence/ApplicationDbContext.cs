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
                    IsActive = true,
                    HashPassword = "+gURQgHBT1zJz5AljZhAMyaNRFQBVorq5HIlEmhf+ZQ=:BBLvXedGXzdz0ZlypoKQxQ==",  // Admin@123
                    Role = UserRole.Admin,
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
                    ApiKey = "api_key value",
                    SecretKey = "secret key value",
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
            base.OnModelCreating(builder);
        }
    }
}
