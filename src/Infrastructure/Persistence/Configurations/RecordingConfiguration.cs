namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;

    public class RecordingConfiguration : IEntityTypeConfiguration<Recording>
    {
        public void Configure(EntityTypeBuilder<Recording> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.LessonId).HasColumnName("lesson_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.VideoUrl).HasColumnName("video_url").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Duration).HasColumnName("duration").HasDefaultValue(0);
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("VARCHAR(150)").HasMaxLength(150).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.Slug).HasColumnName("slug").HasColumnType("VARCHAR(170)").HasMaxLength(170).IsRequired();
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}