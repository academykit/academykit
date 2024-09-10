using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class VideoQueueConfiguration : IEntityTypeConfiguration<VideoQueue>
{
    public void Configure(EntityTypeBuilder<VideoQueue> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(x => x.LessonId)
            .HasColumnName("lesson_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.VideoUrl)
            .HasColumnName("video_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();
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
