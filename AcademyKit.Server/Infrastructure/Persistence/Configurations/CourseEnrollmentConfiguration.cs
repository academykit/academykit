using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class CourseEnrollmentConfiguration : IEntityTypeConfiguration<CourseEnrollment>
{
    public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.CourseId)
            .HasColumnName("course_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.CurrentLessonId)
            .HasColumnName("current_lesson_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50);
        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.CurrentLessonWatched).HasColumnName("current_lesson_watched");
        builder
            .Property(x => x.Percentage)
            .HasColumnName("percentage")
            .HasDefaultValue(0)
            .IsRequired();
        builder.Property(x => x.EnrollmentMemberStatus).HasColumnName("status").IsRequired();
        builder
            .Property(x => x.ActivityReason)
            .HasColumnName("activity_reason")
            .HasColumnType(MigrationConstants.Varchar1000)
            .HasMaxLength(1000)
            .IsRequired(false);
        builder
            .Property(x => x.EnrollmentDate)
            .HasColumnName("enrollment_date")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(true);
        builder
            .Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType("BIT")
            .HasDefaultValue(false);
        builder
            .Property(x => x.DeletedBy)
            .HasColumnName("deleted_by")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        builder
            .Property(x => x.DeletedOn)
            .HasColumnName("deleted_on")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(false);
        builder
            .Property(x => x.HasCertificateIssued)
            .HasColumnName("has_certificate_issued")
            .IsRequired(false);
        builder
            .Property(x => x.CertificateUrl)
            .HasColumnName("certificate_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.CertificateIssuedDate)
            .HasColumnName("certificate_issued_date")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
