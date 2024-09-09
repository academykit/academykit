using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class EligibilityCreationConfiguration : IEntityTypeConfiguration<EligibilityCreation>
{
    public void Configure(EntityTypeBuilder<EligibilityCreation> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.Role).HasColumnName("role");
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
        builder
            .Property(x => x.TrainingId)
            .HasColumnName("training_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        builder
            .Property(x => x.GroupId)
            .HasColumnName("group_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        builder
            .Property(x => x.DepartmentId)
            .HasColumnName("department_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        builder
            .Property(x => x.CompletedAssessmentId)
            .HasColumnName("completed_assessment_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
