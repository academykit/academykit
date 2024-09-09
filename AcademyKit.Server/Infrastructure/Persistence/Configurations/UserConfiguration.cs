using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ConfigureId();
        #region Basic

        builder.Property(x => x.Role).HasColumnName("role").HasDefaultValue(UserRole.Trainee);
        builder
            .Property(x => x.HashPassword)
            .HasColumnName("hash_password")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired(false);
        builder
            .Property(x => x.PublicUrls)
            .HasColumnName("public_urls")
            .HasColumnType(MigrationConstants.Varchar2000)
            .HasMaxLength(2000)
            .IsRequired(false);
        builder
            .Property(x => x.Status)
            .HasColumnName("status")
            .HasDefaultValue(UserStatus.Active)
            .IsRequired();
        builder
            .Property(x => x.PasswordResetToken)
            .HasColumnName("password_reset_token")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        builder
            .Property(x => x.PasswordChangeToken)
            .HasColumnName("password_change_token")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.PasswordResetTokenExpiry)
            .HasColumnName("password_reset_token_expiry")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(false);
        builder
            .Property(x => x.ImageUrl)
            .HasColumnName("image_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.FirstName)
            .HasColumnName("first_name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.MiddleName)
            .HasColumnName("middle_name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired(false);
        builder
            .Property(x => x.LastName)
            .HasColumnName("last_name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        #endregion
        #region Official Info
        builder
            .Property(x => x.MemberId)
            .HasColumnName("member_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .IsRequired(false);
        builder
            .Property(x => x.Profession)
            .HasColumnName("profession")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired(false);
        builder
            .Property(x => x.DepartmentId)
            .HasColumnName("department_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        #endregion
        #region Address
        #region Permanent Address
        builder
            .Property(x => x.Address)
            .HasColumnName("address")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired(false);
        #endregion
        #endregion
        #region Contact Details
        builder
            .Property(x => x.Email)
            .HasColumnName("email")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired(); // Login email (Office Email)
        builder
            .Property(x => x.MobileNumber)
            .HasColumnName("mobile_number")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false); // Primary Mobile number
        #endregion
        builder
            .Property(x => x.Bio)
            .HasColumnName("bio")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.ConfigureAuditFields();

        // foreign relationships configuration
        builder
            .HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Groups)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.GroupMembers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Tags)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Levels)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Courses)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Sections)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Lessons)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Assignments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.CourseTeachers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.ZoomSettings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.ZoomLicenses)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.SMTPSettings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Meetings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.MeetingReports)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.GeneralSettings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Departments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.CourseTags)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSets)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionPools)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Questions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionOptions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionPoolQuestions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetQuestions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionPoolTeachers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetResults)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssessmentResults)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetSubmissions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssessmentSubmissions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetSubmissionAnswers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssessmentSubmissionAnswers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.CourseEnrollments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.WatchHistories)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(x => x.Assessments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentAttachments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentSubmissions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionTags)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentQuestionOptions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentSubmissionAttachments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Comments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.CommentReplies)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.GroupStorages)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentReviews)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Feedbacks)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.FeedbackQuestionOptions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssessmentQuestions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentQuestionOptions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.FeedbackSubmissions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Signatures)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.CourseCertificates)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Certificates)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
