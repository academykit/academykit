namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;
    public class VideoQueueConfiguration : IEntityTypeConfiguration<VideoQueue>
    {
        public void Configure(EntityTypeBuilder<VideoQueue> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.LessonId).HasColumnName("lesson_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.VideoUrl).HasColumnName("video_url").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Status).HasColumnName("status").IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}