namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Lingtren.Domain.Entities;

    public class GroupFileConfiguration : IEntityTypeConfiguration<GroupFile>
    {
        public void Configure(EntityTypeBuilder<GroupFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Url).HasColumnName("url").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired();
            builder.Property(x => x.Key).HasColumnName("key").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired();
            builder.Property(x => x.MimeType).HasColumnName("mime_type").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired();
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}