using AcademyKit.Domain.Entities;
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
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.AssessmentId)
            .HasColumnName("assessment_id")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.AssessmentSubmissionId)
            .HasColumnName("assessment_submission_id")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.TotalMark)
            .HasColumnName("total_mark")
            .HasColumnType("decimal(20,4)");
        builder
            .Property(x => x.NegativeMark)
            .HasColumnName("negative_mark")
            .HasColumnType("decimal(20,4)");

        builder.ConfigureAuditFields();
    }
}
