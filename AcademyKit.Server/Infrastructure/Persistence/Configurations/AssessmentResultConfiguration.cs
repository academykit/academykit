using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    public class AssessmentResultConfiguration : IEntityTypeConfiguration<AssessmentResult>
    {
        public void Configure(EntityTypeBuilder<AssessmentResult> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.AssessmentId)
                .HasColumnName("assessment_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.AssessmentSubmissionId)
                .HasColumnName("assessment_submission_id")
                .HasColumnType("VARCHAR(50)")
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
            builder
                .Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.CreatedOn)
                .HasColumnName("created_on")
                .IsRequired()
                .HasColumnType("DATETIME");
            builder
                .Property(x => x.UpdatedBy)
                .HasColumnName("updated_by")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME");
        }
    }
}
