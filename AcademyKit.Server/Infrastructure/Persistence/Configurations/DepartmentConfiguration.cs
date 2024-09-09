using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
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
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.Users)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(e => e.EligibilityCreations)
            .WithOne(e => e.Department)
            .HasForeignKey(e => e.DepartmentId)
            .IsRequired(false);
    }
}
