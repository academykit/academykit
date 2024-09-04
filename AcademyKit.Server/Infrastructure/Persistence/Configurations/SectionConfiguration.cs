namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Slug)
                .HasColumnName("slug")
                .HasColumnType("varchar(270)")
                .HasMaxLength(270)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(250)")
                .HasMaxLength(250)
                .IsRequired();
            builder
                .Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(x => x.Status).HasColumnName("status");
            builder.Property(x => x.Duration).HasColumnName("duration");
            builder.Property(x => x.Order).HasColumnName("order");
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            builder
                .Property(x => x.CourseId)
                .HasColumnName("course_id")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.Lessons)
                .WithOne(x => x.Section)
                .HasForeignKey(x => x.SectionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
