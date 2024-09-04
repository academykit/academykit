namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ZoomSettingConfiguration : IEntityTypeConfiguration<ZoomSetting>
    {
        public void Configure(EntityTypeBuilder<ZoomSetting> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.SdkKey)
                .HasColumnName("sdk_key")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.OAuthAccountId)
                .HasColumnName("oauth_account_id")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.OAuthClientId)
                .HasColumnName("oauth_client_id")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.OAuthClientSecret)
                .HasColumnName("oauth_client_secret")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.SdkSecret)
                .HasColumnName("sdk_secret")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.WebHookSecret)
                .HasColumnName("webhook_secret")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired(false);
            builder
                .Property(x => x.IsRecordingEnabled)
                .HasColumnName("is_recording_enabled")
                .HasDefaultValue(false);

            builder.ConfigureAuditFields();
        }
    }
}
