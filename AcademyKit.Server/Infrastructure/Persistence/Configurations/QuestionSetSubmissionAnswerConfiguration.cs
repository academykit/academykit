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
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionSetQuestionId)
                .HasColumnName("question_set_question_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionSetSubmissionId)
                .HasColumnName("question_set_submission_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.IsCorrect).HasColumnName("is_correct").HasDefaultValue(false);
            builder
                .Property(x => x.SelectedAnswers)
                .HasColumnName("selected_answers")
                .HasColumnType("VARCHAR(150)")
                .HasMaxLength(150)
                .IsRequired();
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
            builder
                .Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.UpdatedBy)
                .HasColumnName("updated_by")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired(false);
        }
    }
}
