namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
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
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.CourseTags)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
