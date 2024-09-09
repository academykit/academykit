using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class ZoomSettingConfiguration : IEntityTypeConfiguration<ZoomSetting>
{
    public void Configure(EntityTypeBuilder<ZoomSetting> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.SdkKey)
            .HasColumnName("sdk_key")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.OAuthAccountId)
            .HasColumnName("oauth_account_id")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.OAuthClientId)
            .HasColumnName("oauth_client_id")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.OAuthClientSecret)
            .HasColumnName("oauth_client_secret")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.SdkSecret)
            .HasColumnName("sdk_secret")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.WebHookSecret)
            .HasColumnName("webhook_secret")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired(false);
        builder
            .Property(x => x.IsRecordingEnabled)
            .HasColumnName("is_recording_enabled")
            .HasDefaultValue(false);

        builder.ConfigureAuditFields();
    }
}
