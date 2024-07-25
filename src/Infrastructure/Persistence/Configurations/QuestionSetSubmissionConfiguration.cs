namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionSetSubmissionConfiguration
        : IEntityTypeConfiguration<QuestionSetSubmission>
    {
        public void Configure(EntityTypeBuilder<QuestionSetSubmission> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
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
            builder
                .Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.CreatedOn)
                .HasColumnName("created_on")
                .IsRequired()
                .HasColumnType("DATETIME");
            builder
                .Property(x => x.UpdatedBy)
                .HasColumnName("updated_by")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME");
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
