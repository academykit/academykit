using System.Security.Cryptography.X509Certificates;
namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;
    public class LiveSessionConfiguration : IEntityTypeConfiguration<LiveSession>
    {
        public void Configure(EntityTypeBuilder<LiveSession> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Slug).HasColumnName("slug").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Description).HasColumnName("description").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.StartDate).HasColumnName("start_date").HasColumnType("DATETIME");
            builder.Property(x => x.EndDate).HasColumnName("end_date").HasColumnType("DATETIME");
            builder.Property(x => x.EventType).HasColumnName("event_type").IsRequired();
            builder.Property(x => x.Recurrence).HasColumnName("recurrence").HasColumnType("VARCHAR(250)").IsRequired(false);
            builder.Property(x => x.Status).HasColumnName("status");
            builder.Property(x => x.Duration).IsRequired();
            builder.Property(x => x.NearestEndTime).HasColumnName("nearest_end_time").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.NearestStartTime).HasColumnName("nearest_start_time").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.HasMany(x => x.LiveSessionModerators).WithOne(x => x.LiveSession).HasForeignKey(x => x.LiveSessionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.LiveSessionModerators).WithOne(x => x.LiveSession).HasForeignKey(x => x.LiveSessionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Meetings).WithOne(x => x.LiveSession).HasForeignKey(x => x.LiveSessionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.LiveSessionReports).WithOne(x => x.LiveSession).HasForeignKey(x => x.LiveSessionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.LiveSessionMembers).WithOne(x => x.LiveSession).HasForeignKey(x => x.LiveSessionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}