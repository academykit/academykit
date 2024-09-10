using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssessmentSubmissionConfiguration : IEntityTypeConfiguration<AssessmentSubmission>
{
    public void Configure(EntityTypeBuilder<AssessmentSubmission> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.AssessmentId)
            .HasColumnName("assessment_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();

        builder.ConfigureCommonSubmissionProperties();
        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.AssessmentSubmissionAnswers)
            .WithOne(x => x.AssessmentSubmission)
            .HasForeignKey(x => x.AssessmentSubmissionId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssessmentResults)
            .WithOne(x => x.AssessmentSubmission)
            .HasForeignKey(x => x.AssessmentSubmissionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
