namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionSetQuestionConfiguration : IEntityTypeConfiguration<QuestionSetQuestion>
    {
        public void Configure(EntityTypeBuilder<QuestionSetQuestion> builder)
        {
            builder.ConfigureId();

            builder.Property(x => x.Order).HasColumnName("order");
            builder
                .Property(x => x.QuestionSetId)
                .HasColumnName("question_set_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionId)
                .HasColumnName("question_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.QuestionPoolQuestionId)
                .HasColumnName("question_pool_question_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.ConfigureAuditFields();
        }
    }
}
