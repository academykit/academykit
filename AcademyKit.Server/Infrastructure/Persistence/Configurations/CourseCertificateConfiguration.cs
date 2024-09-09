using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class CourseCertificateConfiguration : IEntityTypeConfiguration<CourseCertificate>
{
    public void Configure(EntityTypeBuilder<CourseCertificate> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Title)
            .HasColumnName("title")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.CourseId)
            .HasColumnName("course_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.EventStartDate)
            .HasColumnName("event_start_date")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired();
        builder
            .Property(x => x.EventEndDate)
            .HasColumnName("event_end_date")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired();
        builder
            .Property(x => x.SampleUrl)
            .HasColumnName("sample_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
