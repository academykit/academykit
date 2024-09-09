using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssessmentResultConfiguration : IEntityTypeConfiguration<AssessmentResult>
{
    public void Configure(EntityTypeBuilder<AssessmentResult> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.AssessmentId)
            .HasColumnName("assessment_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.AssessmentSubmissionId)
            .HasColumnName("assessment_submission_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.TotalMark)
            .HasColumnName("total_mark")
            .HasColumnType(MigrationConstants.Decimal20_4);
        builder
            .Property(x => x.NegativeMark)
            .HasColumnName("negative_mark")
            .HasColumnType(MigrationConstants.Decimal20_4);

        builder.ConfigureAuditFields();
    }
}
