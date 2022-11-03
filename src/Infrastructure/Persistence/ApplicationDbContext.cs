using System.Runtime.InteropServices;
namespace Lingtren.Infrastructure.Persistence
{
    using System.Reflection;
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

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
        public DbSet<LiveSession> LiveSessions { get; set; }
        public DbSet<LiveSessionModerator> LiveSessionModerators { get; set; }
        public DbSet<LiveSessionTag> LiveSessionTags { get; set; }
        public DbSet<ZoomSetting> ZoomSettings { get; set; }
        public DbSet<ZoomLicense> ZoomLicenses { get; set; }
        public DbSet<SMTPSetting> SMTPSettings { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<LiveSessionReport> LiveSessionReports { get; set; }
        public DbSet<LiveSessionMember> LiveSessionMembers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
}
