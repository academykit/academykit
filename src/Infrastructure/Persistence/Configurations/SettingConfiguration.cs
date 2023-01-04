namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(x => x.Key);
            builder.Property(x => x.Key).HasColumnName("key").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.Value).HasColumnName("value").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
        }
    }
}