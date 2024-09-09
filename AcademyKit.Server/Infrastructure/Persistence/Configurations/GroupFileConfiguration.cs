using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class GroupFileConfiguration : IEntityTypeConfiguration<GroupFile>
{
    public void Configure(EntityTypeBuilder<GroupFile> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Url)
            .HasColumnName("url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired();
        builder
            .Property(x => x.MimeType)
            .HasColumnName("mime_type")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
