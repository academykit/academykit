namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Persistence.Migrations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.StartDate)
                .HasColumnName("start_date")
                .HasColumnType(MigrationConstants.DateTime)
                .IsRequired(false);
            builder
                .Property(x => x.ZoomLicenseId)
                .HasColumnName("zoom_license_id")
                .HasColumnType(MigrationConstants.Varchar50);
            builder
                .Property(x => x.Passcode)
                .HasColumnName("passcode")
                .HasColumnType(MigrationConstants.Varchar50)
                .HasMaxLength(50)
                .IsRequired(false);
            builder.Property(x => x.Duration).HasColumnName("duration");
            builder
                .Property(x => x.MeetingNumber)
                .HasColumnName("meeting_number")
                .IsRequired(false);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.MeetingReports)
                .WithOne(x => x.Meeting)
                .HasForeignKey(x => x.MeetingId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(x => x.Lesson)
                .WithOne(x => x.Meeting)
                .HasForeignKey<Lesson>(x => x.MeetingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
