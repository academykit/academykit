using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class SkillsConfiguration : IEntityTypeConfiguration<Skills>
{
    public void Configure(EntityTypeBuilder<Skills> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.SkillName)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired(true);

        builder
            .Property(x => x.Description)
            .HasColumnName("remarks")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);

        builder.ConfigureAuditFields();

        builder
            .HasMany(e => e.EligibilityCreations)
            .WithOne(e => e.Skills)
            .HasForeignKey(e => e.SkillId)
            .IsRequired(false);
        builder
            .HasMany(e => e.SkillsCriteria)
            .WithOne(e => e.SkillType)
            .HasForeignKey(e => e.SkillId)
            .IsRequired(false);
    }
}
