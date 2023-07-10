using Lingtren.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lingtren.Infrastructure.Persistence.Configurations
{
    public class LogsConfiguration : IEntityTypeConfiguration<Logs>
    {
        public void Configure(EntityTypeBuilder<Logs> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.MachineName).HasColumnType("VARCHAR(200)").IsRequired(false);
            builder.Property(x => x.Logged).HasColumnType("DATETIME").IsRequired(true);
            builder.Property(x => x.Level).HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.Message).HasColumnType("VARCHAR(4000)").IsRequired(false);
            builder.Property(x => x.Logger).HasColumnType("VARCHAR(400)").IsRequired(false);
            builder.Property(x => x.Properties).HasColumnType("VARCHAR(1000)").IsRequired(false);
            builder.Property(x => x.Exception).HasColumnType("VARCHAR(5000)").IsRequired(false);
        }
    }
}
