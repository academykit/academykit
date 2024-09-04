namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class LicenseConfiguration : IEntityTypeConfiguration<License>
    {
        public void Configure(EntityTypeBuilder<License> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.licenseKey)
                .HasColumnName("licenseKey")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.status).HasColumnName("status").IsRequired();
            builder
                .Property(x => x.licenseKeyId)
                .HasColumnName("licenseKeyId")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.customerName)
                .HasColumnName("customer_name")
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.customerEmail)
                .HasColumnName("customer_email")
                .HasColumnType("varchar(200)")
                .HasMaxLength(200)
                .IsRequired();

            builder.ConfigureAuditFields();
        }
    }
}
