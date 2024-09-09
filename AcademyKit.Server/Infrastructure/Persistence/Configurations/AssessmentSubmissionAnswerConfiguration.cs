using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssessmentSubmissionAnswerConfiguration
    : IEntityTypeConfiguration<AssessmentSubmissionAnswer>
{
    public void Configure(EntityTypeBuilder<AssessmentSubmissionAnswer> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.AssessmentSubmissionId)
            .HasColumnName("assessment_submission_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(x => x.AssessmentQuestionId)
            .HasColumnName("assessment_question_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsCorrect).HasColumnName("is_correct").HasDefaultValue(false);

        builder
            .Property(x => x.SelectedAnswers)
            .HasColumnName("selected_answers")
            .HasColumnType(MigrationConstants.Varchar150)
            .HasMaxLength(150)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
