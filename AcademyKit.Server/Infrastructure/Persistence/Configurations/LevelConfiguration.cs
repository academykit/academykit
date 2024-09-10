using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Slug)
            .HasColumnName("slug")
            .HasColumnType(MigrationConstants.Varchar270)
            .HasMaxLength(250)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.Courses)
            .WithOne(x => x.Level)
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
