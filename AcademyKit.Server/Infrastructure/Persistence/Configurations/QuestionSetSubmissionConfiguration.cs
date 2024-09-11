using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class QuestionSetSubmissionConfiguration : IEntityTypeConfiguration<QuestionSetSubmission>
{
    public void Configure(EntityTypeBuilder<QuestionSetSubmission> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.QuestionSetId)
            .HasColumnName("question_set_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.ConfigureCommonSubmissionProperties();
        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.QuestionSetResults)
            .WithOne(x => x.QuestionSetSubmission)
            .HasForeignKey(x => x.QuestionSetSubmissionId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetSubmissionAnswers)
            .WithOne(x => x.QuestionSetSubmission)
            .HasForeignKey(x => x.QuestionSetSubmissionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
