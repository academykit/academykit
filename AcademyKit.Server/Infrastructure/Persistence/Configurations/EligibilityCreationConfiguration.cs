namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EligibilityCreationConfiguration : IEntityTypeConfiguration<EligibilityCreation>
    {
        public void Configure(EntityTypeBuilder<EligibilityCreation> builder)
        {
            builder.ConfigureId();

            builder.Property(x => x.Role).HasColumnName("role");
            builder
                .Property(x => x.SkillId)
                .HasColumnName("skill_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.AssessmentId)
                .HasColumnName("assessment_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.TrainingId)
                .HasColumnName("training_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.GroupId)
                .HasColumnName("group_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.DepartmentId)
                .HasColumnName("department_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.CompletedAssessmentId)
                .HasColumnName("completed_assessment_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.ConfigureAuditFields();
        }
    }
}
