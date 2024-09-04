namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SignatureConfiguration : IEntityTypeConfiguration<Signature>
    {
        public void Configure(EntityTypeBuilder<Signature> builder)
        {
            builder.ConfigureId();
            builder
                .Property(x => x.CourseId)
                .HasColumnName("course_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Designation)
                .HasColumnName("designation")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.FullName)
                .HasColumnName("full_name")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.FileUrl)
                .HasColumnName("file_url")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();

            builder.ConfigureAuditFields();
        }
    }
}
