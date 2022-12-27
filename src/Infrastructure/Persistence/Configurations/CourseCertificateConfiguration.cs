namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class CourseCertificateConfiguration : IEntityTypeConfiguration<CourseCertificate>
    {
        public void Configure(EntityTypeBuilder<CourseCertificate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Title).HasColumnName("title").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.CourseId).HasColumnName("course_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.EventStartDate).HasColumnName("event_start_date").HasColumnType("DATETIME").IsRequired();
            builder.Property(x => x.EventEndDate).HasColumnName("event_end_date").HasColumnType("DATETIME").IsRequired();
            builder.Property(x => x.SampleUrl).HasColumnName("sample_url").HasColumnType("VARCHAR(500)").IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}
