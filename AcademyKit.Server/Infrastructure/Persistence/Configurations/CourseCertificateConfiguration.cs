namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CourseCertificateConfiguration : IEntityTypeConfiguration<CourseCertificate>
    {
        public void Configure(EntityTypeBuilder<CourseCertificate> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.CourseId)
                .HasColumnName("course_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.EventStartDate)
                .HasColumnName("event_start_date")
                .HasColumnType("DATETIME")
                .IsRequired();
            builder
                .Property(x => x.EventEndDate)
                .HasColumnName("event_end_date")
                .HasColumnType("DATETIME")
                .IsRequired();
            builder
                .Property(x => x.SampleUrl)
                .HasColumnName("sample_url")
                .HasColumnType("VARCHAR(500)")
                .IsRequired(false);

            builder.ConfigureAuditFields();
        }
    }
}
