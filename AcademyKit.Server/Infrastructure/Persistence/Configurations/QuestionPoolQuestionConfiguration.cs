namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionPoolQuestionConfiguration : IEntityTypeConfiguration<QuestionPoolQuestion>
    {
        public void Configure(EntityTypeBuilder<QuestionPoolQuestion> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.QuestionId)
                .HasColumnName("question_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionPoolId)
                .HasColumnName("question_pool_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.QuestionSetQuestions)
                .WithOne(x => x.QuestionPoolQuestion)
                .HasForeignKey(x => x.QuestionPoolQuestionId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(x => x.Order).HasColumnName("order").HasDefaultValue(0);
        }
    }
}
