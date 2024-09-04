namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.LessonId)
                .HasColumnName("lesson_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("VARCHAR(500)")
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(x => x.Order).HasColumnName("order").HasDefaultValue(0);
            builder.Property(x => x.Type).HasColumnName("type").IsRequired();
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.FeedbackSubmissions)
                .WithOne(x => x.Feedback)
                .HasForeignKey(x => x.FeedbackId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.FeedbackQuestionOptions)
                .WithOne(x => x.Feedback)
                .HasForeignKey(x => x.FeedbackId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
