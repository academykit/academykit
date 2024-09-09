using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class GeneralSettingConfiguration : IEntityTypeConfiguration<GeneralSetting>
{
    public void Configure(EntityTypeBuilder<GeneralSetting> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.LogoUrl)
            .HasColumnName("logo_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.CompanyName)
            .HasColumnName("company_name")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired();
        builder
            .Property(x => x.CompanyAddress)
            .HasColumnName("company_address")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired(false);
        builder
            .Property(x => x.CompanyContactNumber)
            .HasColumnName("company_contact_number")
            .HasColumnType(MigrationConstants.Varchar30)
            .HasMaxLength(30)
            .IsRequired(false);
        builder
            .Property(x => x.EmailSignature)
            .HasColumnName("email_signature")
            .HasColumnType(MigrationConstants.Varchar1000)
            .HasMaxLength(1000)
            .IsRequired(false);
        builder.Property(x => x.IsSetupCompleted).HasColumnName("is_setup_completed");
        builder
            .Property(x => x.CustomConfiguration)
            .HasColumnName("custom_configuration")
            .HasColumnType(MigrationConstants.Varchar5000)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
