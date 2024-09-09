using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.Token)
            .HasColumnName("token")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired(false);
        builder.Property(x => x.LoginAt).HasColumnName("login_at").HasColumnType("DATETIME");
        builder
            .Property(x => x.DeviceId)
            .HasColumnName("device_id")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired(false);
        builder
            .Property(x => x.Location)
            .HasColumnName("location")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired(false);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

        builder.ConfigureAuditFields();
    }
}
