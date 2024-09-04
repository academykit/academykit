namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionSetSubmissionConfiguration
        : IEntityTypeConfiguration<QuestionSetSubmission>
    {
        public void Configure(EntityTypeBuilder<QuestionSetSubmission> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.QuestionSetId)
                .HasColumnName("question_set_id")
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
                .Property(x => x.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("DATETIME");
            builder.Property(x => x.EndTime).HasColumnName("end_time").HasColumnType("DATETIME");
            builder
                .Property(x => x.IsSubmissionError)
                .HasColumnName("is_submission_error")
                .HasDefaultValue(false);
            builder
                .Property(x => x.SubmissionErrorMessage)
                .HasColumnName("submission_error_message")
                .HasColumnType("VARCHAR(250)")
                .HasMaxLength(250)
                .IsRequired(false);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.QuestionSetResults)
                .WithOne(x => x.QuestionSetSubmission)
                .HasForeignKey(x => x.QuestionSetSubmissionId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.QuestionSetSubmissionAnswers)
                .WithOne(x => x.QuestionSetSubmission)
                .HasForeignKey(x => x.QuestionSetSubmissionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
