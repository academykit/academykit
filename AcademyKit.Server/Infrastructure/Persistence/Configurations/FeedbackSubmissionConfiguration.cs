using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class FeedbackSubmissionConfiguration : IEntityTypeConfiguration<FeedbackSubmission>
{
    public void Configure(EntityTypeBuilder<FeedbackSubmission> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.LessonId)
            .HasColumnName("lesson_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.FeedbackId)
            .HasColumnName("feedback_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
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
        builder.Property(x => x.Rating).HasColumnName("rating").IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
