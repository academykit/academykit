namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class LicenseConfiguration : IEntityTypeConfiguration<License>
    {
        public void Configure(EntityTypeBuilder<License> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.licenseKey)
                .HasColumnName("licenseKey")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.status).HasColumnName("status").IsRequired();
            builder
                .Property(x => x.licenseKeyId)
                .HasColumnName("licenseKeyId")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.customerName)
                .HasColumnName("customerName")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.customerEmail)
                .HasColumnName("customerEmail")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}

