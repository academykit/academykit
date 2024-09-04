namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SkillsCriteriaConfiguration : IEntityTypeConfiguration<SkillsCriteria>
    {
        public void Configure(EntityTypeBuilder<SkillsCriteria> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Percentage)
                .HasColumnName("percentage")
                .HasColumnType("decimal(20,4)");
            builder.Property(x => x.SkillAssessmentRule).HasColumnName("skill_assessment_rule");

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

            builder.ConfigureAuditFields();
        }
    }
}
