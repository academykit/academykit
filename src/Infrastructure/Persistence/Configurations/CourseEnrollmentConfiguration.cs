namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class CourseEnrollmentConfiguration : IEntityTypeConfiguration<CourseEnrollment>
    {
        public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CourseId).HasColumnName("course_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CurrentLessonId).HasColumnName("current_lesson_id").HasColumnType("VARCHAR(50)").HasMaxLength(50);
            builder.Property(x => x.UserId).HasColumnName("user_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CurrentLessonWatched).HasColumnName("current_lesson_watched");
            builder.Property(x => x.Percentage).HasColumnName("percentage").HasDefaultValue(0).IsRequired();
            builder.Property(x => x.EnrollmentMemberStatus).HasColumnName("status").IsRequired();
            builder.Property(x => x.ActivityReason).HasColumnName("activity_reason").HasColumnType("VARCHAR(1000)").HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.EnrollmentDate).HasColumnName("enrollment_date").HasColumnType("DATETIME").IsRequired(true);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME");
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasColumnType("BIT").HasDefaultValue(false);
            builder.Property(x => x.DeletedBy).HasColumnName("deleted_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.DeletedOn).HasColumnName("deleted_on").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.HasCertificateIssued).HasColumnName("has_certificate_issued").IsRequired(false);
            builder.Property(x => x.CertificateUrl).HasColumnName("certificate_url").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.CertificateIssuedDate).HasColumnName("certificate_issued_date").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}
