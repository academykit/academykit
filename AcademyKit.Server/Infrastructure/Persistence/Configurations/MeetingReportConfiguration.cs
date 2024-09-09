namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Persistence.Migrations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MeetingReportConfiguration : IEntityTypeConfiguration<MeetingReport>
    {
        public void Configure(EntityTypeBuilder<MeetingReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(MigrationConstants.Varchar50)
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.MeetingId)
                .HasColumnName("meeting_id")
                .HasColumnType(MigrationConstants.Varchar50)
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType(MigrationConstants.Varchar50)
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.StartTime)
                .HasColumnName("start_time")
                .HasColumnType(MigrationConstants.DateTime)
                .IsRequired();
            builder
                .Property(x => x.JoinTime)
                .HasColumnName("join_time")
                .HasColumnType(MigrationConstants.DateTime)
                .IsRequired();
            builder
                .Property(x => x.LeftTime)
                .HasColumnName("left_time")
                .HasColumnType(MigrationConstants.DateTime)
                .IsRequired(false);
            builder.Property(x => x.Duration).HasColumnName("duration").IsRequired(false);
            builder
                .Property(x => x.CreatedOn)
                .HasColumnName("created_on")
                .IsRequired()
                .HasColumnType(MigrationConstants.DateTime);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType(MigrationConstants.DateTime)
                .IsRequired(false);
        }
    }
}
