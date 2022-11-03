namespace Lingtren.Infrastructure.Persistence
{
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Persistence.Configurations;
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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RefreshTokenConfiguration());
            builder.ApplyConfiguration(new GroupConfiguration());
            builder.ApplyConfiguration(new GroupMemberConfiguration());
            builder.ApplyConfiguration(new TagConfiguration());
            builder.ApplyConfiguration(new LevelConfiguration());
            builder.ApplyConfiguration(new CourseConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
