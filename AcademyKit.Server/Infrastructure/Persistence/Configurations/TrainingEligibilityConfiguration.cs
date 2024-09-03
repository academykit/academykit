namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TrainingEligibilityConfiguration : IEntityTypeConfiguration<TrainingEligibility>
    {
        public void Configure(EntityTypeBuilder<TrainingEligibility> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(x => x.CourseId)
                .HasColumnName("course_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.EligibilityId)
                .HasColumnName("eligibility_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.TrainingEligibilityEnum)
                .HasColumnName("training_eligibility_enum")
                .HasColumnType("int")
                .IsRequired();

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
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME")
                .IsRequired(false);
        }
    }
}
