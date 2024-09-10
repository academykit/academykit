﻿using AcademyKit.Domain.Entities;
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
            .Property(x => x.licenseKey)
            .HasColumnName("licenseKey")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.status).HasColumnName("status").IsRequired();
        builder
            .Property(x => x.licenseKeyId)
            .HasColumnName("licenseKeyId")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.customerName)
            .HasColumnName("customer_name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.customerEmail)
            .HasColumnName("customer_email")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
