using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired(false);
        builder
            .Property(x => x.Key)
            .HasColumnName("key")
            .HasColumnType(MigrationConstants.Varchar64)
            .HasMaxLength(64)
            .IsRequired(true);

        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
