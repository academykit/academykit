using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.LessonId)
            .HasColumnName("lesson_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.AssignmentId)
            .HasColumnName("assignment_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.IsCorrect).HasColumnName("is_correct").HasDefaultValue(false);
        builder
            .Property(x => x.SelectedOption)
            .HasColumnName("selected_option")
            .HasColumnType(MigrationConstants.Varchar300)
            .HasMaxLength(300)
            .IsRequired(false);
        builder
            .Property(x => x.Answer)
            .HasColumnName("answer")
            .HasColumnType(MigrationConstants.Varchar5000)
            .HasMaxLength(5000)
            .IsRequired(false);

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.AssignmentSubmissionAttachments)
            .WithOne(x => x.AssignmentSubmission)
            .HasForeignKey(x => x.AssignmentSubmissionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
