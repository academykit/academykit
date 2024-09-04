using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    public class LogsConfiguration : IEntityTypeConfiguration<Logs>
    {
        public void Configure(EntityTypeBuilder<Logs> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.MachineName).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(x => x.Logged).HasColumnType("DATETIME").IsRequired(true);
            builder
                .Property(x => x.Level)
                .HasColumnType("varchar(200)")
                .HasMaxLength(200)
                .IsRequired(false);
            builder.Property(x => x.Message).HasColumnType("varchar(4000)").IsRequired(false);
            builder.Property(x => x.Logger).HasColumnType("varchar(400)").IsRequired(false);
            builder.Property(x => x.Properties).HasColumnType("varchar(1000)").IsRequired(false);
            builder.Property(x => x.Exception).HasColumnType("varchar(5000)").IsRequired(false);
        }
    }
}
