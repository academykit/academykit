namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Slug)
                .HasColumnName("slug")
                .HasColumnType("VARCHAR(270)")
                .HasMaxLength(270)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("VARCHAR(250)")
                .HasMaxLength(250)
                .IsRequired();
            builder
                .Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("VARCHAR(5000)")
                .HasMaxLength(5000)
                .IsRequired(false);
            builder
                .Property(x => x.GroupId)
                .HasColumnName("group_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder.Property(x => x.Status).HasColumnName("status");
            builder.Property(x => x.IsUpdate).HasColumnName("is_update").HasDefaultValue(false);
            builder
                .Property(x => x.IsUnlimitedEndDate)
                .HasColumnName("is_unlimited_end_date")
                .HasDefaultValue(false);
            builder.Property(x => x.Language).HasColumnName("language");
            builder
                .Property(x => x.ThumbnailUrl)
                .HasColumnName("thumbnail_url")
                .HasColumnType("VARCHAR(500)")
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(x => x.Duration).HasColumnName("duration");
            builder
                .Property(x => x.LevelId)
                .HasColumnName("level_id")
                .HasColumnType("VARCHAR(50)")
                .IsRequired();
            builder
                .Property(x => x.StartDate)
                .HasColumnName("start_date")
                .HasColumnType("DATETIME")
                .IsRequired(false);
            builder
                .Property(x => x.EndDate)
                .HasColumnName("end_date")
                .HasColumnType("DATETIME")
                .IsRequired(false);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.Sections)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.Lessons)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.CourseTeachers)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.CourseTags)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.WatchHistories)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany(x => x.CourseEnrollments)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany(x => x.Signatures)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany(x => x.TrainingEligibilities)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(x => x.CourseCertificate)
                .WithOne(x => x.Course)
                .HasForeignKey<CourseCertificate>(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(x => x.CourseCertificate)
                .WithOne(x => x.Course)
                .HasForeignKey<CourseCertificate>(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(e => e.EligibilityCreations)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.TrainingId)
                .IsRequired(false);
        }
    }
}
