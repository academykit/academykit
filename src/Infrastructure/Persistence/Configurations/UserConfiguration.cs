namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.FirstName).HasColumnName("first_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.MiddleName).HasColumnName("middle_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.LastName).HasColumnName("last_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasColumnName("email").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.MobileNumber).HasColumnName("mobile_number").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Role).HasColumnName("role").HasDefaultValue(UserRole.User);
            builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Profession).HasColumnName("profession").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.Address).HasColumnName("address").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Bio).HasColumnName("bio").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.HashPassword).HasColumnName("hash_password").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.PublicUrls).HasColumnName("public_urls").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);
            builder.Property(x => x.PasswordResetToken).HasColumnName("password_reset_token").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.PasswordChangeToken).HasColumnName("password_change_token").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.PasswordResetTokenExpiry).HasColumnName("password_reset_token_expiry").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.DepartmentId).HasColumnName("department_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.HasMany(x => x.RefreshTokens).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Groups).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.GroupMembers).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Tags).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Levels).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Courses).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Sections).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Lessons).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseTeachers).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.ZoomSettings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.ZoomLicenses).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.SMTPSettings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Meetings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.MeetingReports).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.GeneralSettings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Departments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseTags).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSets).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionPools).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Questions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionOptions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionPoolQuestions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetQuestions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionPoolTeachers).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetResults).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetSubmissions).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetSubmissionAnswers).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseEnrollments).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.WatchHistories).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Assignments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentAttachments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentQuestions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentMCQSubmissions).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionTags).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
        }
    }
}