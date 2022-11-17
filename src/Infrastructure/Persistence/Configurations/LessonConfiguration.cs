namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Slug).HasColumnName("slug").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Description).HasColumnName("description").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.Status).HasColumnName("status");
            builder.Property(x => x.Duration).HasColumnName("duration");
            builder.Property(x => x.Order).HasColumnName("order");
            builder.Property(x => x.Type).HasColumnName("type").IsRequired();
            builder.Property(x => x.DocumentUrl).HasColumnName("document_url").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.IsPreview).HasColumnName("is_preview").HasDefaultValue(false);
            builder.Property(x => x.IsMandatory).HasColumnName("is_mandatory").HasDefaultValue(false);
            builder.Property(x => x.VideoUrl).HasColumnName("video_url").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.ThumbnailUrl).HasColumnName("thumbnail_url").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            builder.Property(x => x.CourseId).HasColumnName("course_id").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(x => x.SectionId).HasColumnName("section_id").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(x => x.MeetingId).HasColumnName("meeting_id").HasColumnType("VARCHAR(50)").IsRequired(false);
            builder.Property(x => x.QuestionSetId).HasColumnName("question_set_id").HasColumnType("VARCHAR(50)").IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.HasMany(x => x.WatchHistories).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseEnrollments).WithOne(x => x.Lesson).HasForeignKey(x => x.CurrentLessonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Assignments).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Recordings).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}