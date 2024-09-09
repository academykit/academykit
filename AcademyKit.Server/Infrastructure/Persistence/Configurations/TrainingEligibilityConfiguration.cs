namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Persistence.Migrations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TrainingEligibilityConfiguration : IEntityTypeConfiguration<TrainingEligibility>
    {
        public void Configure(EntityTypeBuilder<TrainingEligibility> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.CourseId)
                .HasColumnName("course_id")
                .HasColumnType(MigrationConstants.Varchar50)
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.EligibilityId)
                .HasColumnName("eligibility_id")
                .HasColumnType(MigrationConstants.Varchar50)
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.TrainingEligibilityEnum)
                .HasColumnName("training_eligibility_enum")
                .HasColumnType(MigrationConstants.Int)
                .IsRequired();

            builder.ConfigureAuditFields();
        }
    }
}
