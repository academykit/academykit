namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ZoomLicenseConfiguration : IEntityTypeConfiguration<ZoomLicense>
    {
        public void Configure(EntityTypeBuilder<ZoomLicense> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.LicenseEmail)
                .HasColumnName("license_email")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.HostId)
                .HasColumnName("host_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            builder.Property(x => x.Capacity).HasColumnName("capacity").IsRequired();

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.Meetings)
                .WithOne(x => x.ZoomLicense)
                .HasForeignKey(x => x.ZoomLicenseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
