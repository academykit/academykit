using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();
        builder
            .Property(x => x.StartDate)
            .HasColumnName("start_date")
            .HasColumnType(MigrationConstants.DateTime);
        builder
            .Property(x => x.EndDate)
            .HasColumnName("end_date")
            .HasColumnType(MigrationConstants.DateTime);
        builder
            .Property(x => x.ImageUrl)
            .HasColumnName("image_url")
            .HasColumnType(MigrationConstants.Varchar200)
            .IsRequired(false);
        builder
            .Property(x => x.Location)
            .HasColumnName("location")
            .HasColumnType(MigrationConstants.Varchar100)
            .IsRequired(false);
        builder
            .Property(x => x.Institute)
            .HasColumnName("institute")
            .HasColumnType(MigrationConstants.Varchar100)
            .IsRequired(false);
        builder.Property(x => x.Duration).HasColumnName("duration").IsRequired(false);
        builder
            .Property(x => x.OptionalCost)
            .HasColumnName("optional_cost")
            .HasColumnType(MigrationConstants.Decimal10_2)
            .HasDefaultValue(0);
        builder.Property(x => x.Status).HasColumnName("status");

        builder.ConfigureAuditFields();
    }
}
