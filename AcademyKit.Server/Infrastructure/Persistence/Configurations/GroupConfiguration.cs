using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Slug)
            .HasColumnName("slug")
            .HasColumnType(MigrationConstants.Varchar270)
            .HasMaxLength(270)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired();
        builder.Property(x => x.IsDefault).HasColumnName("is_default").HasDefaultValue(false);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.GroupMembers)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(x => x.GroupFiles)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(e => e.EligibilityCreations)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId)
            .IsRequired(false);
    }
}
