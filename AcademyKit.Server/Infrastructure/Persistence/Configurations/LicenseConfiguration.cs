using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.LicenseKey)
            .HasColumnName("licenseKey")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();
        builder
            .Property(x => x.LicenseKeyId)
            .HasColumnName("licenseKeyId")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.CustomerName)
            .HasColumnName("customer_name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.CustomerEmail)
            .HasColumnName("customer_email")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(x => x.ActivatedOn)
            .HasColumnName("activation_on")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired();

        builder
            .Property(x => x.ExpiredOn)
            .HasColumnName("expired_on")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired();

        builder
            .Property(x => x.VariantName)
            .HasColumnName("variant_name")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(x => x.VariantId)
            .HasColumnName("variant_id")
            .HasColumnType(MigrationConstants.Int)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
