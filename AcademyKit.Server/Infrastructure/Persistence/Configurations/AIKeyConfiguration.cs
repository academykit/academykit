using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AIKeyConfiguration : IEntityTypeConfiguration<AIKey>
{
    public void Configure(EntityTypeBuilder<AIKey> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Key)
            .HasColumnName("key")
            .HasColumnType(MigrationConstants.Varchar270)
            .HasMaxLength(270)
            .IsRequired(true);

        builder.Property(x => x.AiModel).HasColumnName("ai_model");

        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);

        builder.ConfigureAuditFields();
    }
}
