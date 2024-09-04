namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class FeedbackSubmissionConfiguration : IEntityTypeConfiguration<FeedbackSubmission>
    {
        public void Configure(EntityTypeBuilder<FeedbackSubmission> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.LessonId)
                .HasColumnName("lesson_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.FeedbackId)
                .HasColumnName("feedback_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.SelectedOption)
                .HasColumnName("selected_option")
                .HasColumnType("VARCHAR(300)")
                .HasMaxLength(300)
                .IsRequired(false);
            builder
                .Property(x => x.Answer)
                .HasColumnName("answer")
                .HasColumnType("VARCHAR(5000)")
                .HasMaxLength(5000)
                .IsRequired(false);
            builder.Property(x => x.Rating).HasColumnName("rating").IsRequired(false);

            builder.ConfigureAuditFields();
        }
    }
}
