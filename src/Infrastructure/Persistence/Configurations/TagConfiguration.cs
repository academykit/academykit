namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Slug).HasColumnName("slug").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}