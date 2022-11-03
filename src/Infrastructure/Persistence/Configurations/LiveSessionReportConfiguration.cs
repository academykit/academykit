namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;
    public class LiveSessionReportConfiguration : IEntityTypeConfiguration<LiveSessionReport>
    {
        public void Configure(EntityTypeBuilder<LiveSessionReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.LiveSessionId).HasColumnName("live_session_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.MeetingId).HasColumnName("meeting_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.UserId).HasColumnName("user_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.StartTime).HasColumnName("start_time").HasColumnType("DATETIME").IsRequired();
            builder.Property(x => x.JoinTime).HasColumnName("join_time").HasColumnType("DATETIME").IsRequired();
            builder.Property(x => x.LeftTime).HasColumnName("left_time").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.Duration).HasColumnName("duration").IsRequired(false);
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}