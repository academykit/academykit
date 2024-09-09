using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Slug)
            .HasColumnName("slug")
            .HasColumnType(MigrationConstants.Varchar270)
            .HasMaxLength(270)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired();
        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(MigrationConstants.Varchar5000)
            .HasMaxLength(5000)
            .IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status");
        builder.Property(x => x.Duration).HasColumnName("duration");
        builder.Property(x => x.Order).HasColumnName("order");
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder
            .Property(x => x.StartDate)
            .HasColumnName("start_date")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(false);
        builder
            .Property(x => x.EndDate)
            .HasColumnName("end_date")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(false);
        builder
            .Property(x => x.DocumentUrl)
            .HasColumnName("document_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(x => x.IsMandatory).HasColumnName("is_mandatory").HasDefaultValue(false);
        builder
            .Property(x => x.VideoUrl)
            .HasColumnName("video_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.ThumbnailUrl)
            .HasColumnName("thumbnail_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.ExternalUrl)
            .HasColumnName("external_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.Content)
            .HasColumnName("content")
            .HasColumnType(MigrationConstants.Varchar5000)
            .HasMaxLength(5000)
            .IsRequired(false);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder
            .Property(x => x.VideoKey)
            .HasColumnName("video_key")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired(false);
        builder
            .Property(x => x.CourseId)
            .HasColumnName("course_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .IsRequired();
        builder
            .Property(x => x.SectionId)
            .HasColumnName("section_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .IsRequired();
        builder
            .Property(x => x.MeetingId)
            .HasColumnName("meeting_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .IsRequired(false);
        builder
            .Property(x => x.QuestionSetId)
            .HasColumnName("question_set_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .IsRequired(false);

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.WatchHistories)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.CourseEnrollments)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.CurrentLessonId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.Assignments)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentSubmissions)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.AssignmentReviews)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.physicalLessonReviews)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
