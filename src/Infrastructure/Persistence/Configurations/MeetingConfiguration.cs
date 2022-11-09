using System.ComponentModel;
namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.StartDate).HasColumnName("start_date").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.ZoomLicenseId).HasColumnName("zoom_license_id").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(x => x.PassCode).HasColumnName("passcode").HasColumnType("VARCHAR(50)").HasMaxLength(50);
            builder.Property(x => x.Duration).HasColumnName("duration");
            builder.Property(x => x.MeetingNumber).HasColumnName("meeting_number");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.HasMany(x => x.MeetingReports).WithOne(x => x.Meeting).HasForeignKey(x => x.MeetingId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Lessons).WithOne(x => x.Meeting).HasForeignKey(x => x.MeetingId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}