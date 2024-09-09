using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class SkillsCriteriaConfiguration : IEntityTypeConfiguration<SkillsCriteria>
{
    public void Configure(EntityTypeBuilder<SkillsCriteria> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Percentage)
            .HasColumnName("percentage")
            .HasColumnType(MigrationConstants.Decimal20_4);
        builder.Property(x => x.SkillAssessmentRule).HasColumnName("skill_assessment_rule");

        builder
            .Property(x => x.SkillId)
            .HasColumnName("skill_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        builder
            .Property(x => x.AssessmentId)
            .HasColumnName("assessment_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
