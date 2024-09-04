namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionSetSubmissionAnswerConfiguration
        : IEntityTypeConfiguration<QuestionSetSubmissionAnswer>
    {
        public void Configure(EntityTypeBuilder<QuestionSetSubmissionAnswer> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.QuestionSetQuestionId)
                .HasColumnName("question_set_question_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionSetSubmissionId)
                .HasColumnName("question_set_submission_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.IsCorrect).HasColumnName("is_correct").HasDefaultValue(false);
            builder
                .Property(x => x.SelectedAnswers)
                .HasColumnName("selected_answers")
                .HasColumnType("varchar(150)")
                .HasMaxLength(150)
                .IsRequired();

            builder.ConfigureAuditFields();
        }
    }
}
