namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(x => x.Key);
            builder
                .Property(x => x.Key)
                .HasColumnName("key")
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.Value)
                .HasColumnName("value")
                .HasColumnType("varchar(200)")
                .HasMaxLength(200)
                .IsRequired(false);
        }
    }
}
