namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class VideoQueueConfiguration : IEntityTypeConfiguration<VideoQueue>
    {
        public void Configure(EntityTypeBuilder<VideoQueue> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.LessonId)
                .HasColumnName("lesson_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.VideoUrl)
                .HasColumnName("video_url")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(x => x.Status).HasColumnName("status").IsRequired();
            builder
                .Property(x => x.CreatedOn)
                .HasColumnName("created_on")
                .IsRequired()
                .HasColumnType("DATETIME");
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME")
                .IsRequired(false);
        }
    }
}
