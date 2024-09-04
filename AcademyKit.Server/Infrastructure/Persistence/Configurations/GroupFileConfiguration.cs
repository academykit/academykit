namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class GroupFileConfiguration : IEntityTypeConfiguration<GroupFile>
    {
        public void Configure(EntityTypeBuilder<GroupFile> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Url)
                .HasColumnName("url")
                .HasColumnType("VARCHAR(500)")
                .HasMaxLength(500)
                .IsRequired();
            builder
                .Property(x => x.MimeType)
                .HasColumnName("mime_type")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();

            builder.ConfigureAuditFields();
        }
    }
}
