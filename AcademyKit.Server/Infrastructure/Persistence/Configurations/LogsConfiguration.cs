using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class LogsConfiguration : IEntityTypeConfiguration<Logs>
{
    public void Configure(EntityTypeBuilder<Logs> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder
            .Property(x => x.MachineName)
            .HasColumnType(MigrationConstants.Varchar200)
            .IsRequired(false);
        builder.Property(x => x.Logged).HasColumnType(MigrationConstants.DateTime).IsRequired(true);
        builder
            .Property(x => x.Level)
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired(false);
        builder
            .Property(x => x.Message)
            .HasColumnType(MigrationConstants.Varchar4000)
            .IsRequired(false);
        builder
            .Property(x => x.Logger)
            .HasColumnType(MigrationConstants.Varchar400)
            .IsRequired(false);
        builder
            .Property(x => x.Properties)
            .HasColumnType(MigrationConstants.Varchar1000)
            .IsRequired(false);
        builder
            .Property(x => x.Exception)
            .HasColumnType(MigrationConstants.Varchar5000)
            .IsRequired(false);
    }
}
