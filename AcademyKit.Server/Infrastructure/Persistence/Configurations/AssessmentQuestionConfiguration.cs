using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssessmentQuestionConfiguration : IEntityTypeConfiguration<AssessmentQuestion>
{
    public void Configure(EntityTypeBuilder<AssessmentQuestion> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.AssessmentId)
            .HasColumnName("assessment_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(5000)
            .IsRequired(false);
        builder.Property(x => x.Hints).HasColumnName("hints").HasMaxLength(5000).IsRequired(false);
        builder.Property(x => x.Order).HasColumnName("order").HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.AssessmentOptions)
            .WithOne(x => x.AssessmentQuestion)
            .HasForeignKey(x => x.AssessmentQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(x => x.AssessmentSubmissionAnswers)
            .WithOne(x => x.AssessmentQuestion)
            .HasForeignKey(x => x.AssessmentQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
