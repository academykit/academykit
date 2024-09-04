using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    public class QuestionSetResultConfiguration : IEntityTypeConfiguration<QuestionSetResult>
    {
        public void Configure(EntityTypeBuilder<QuestionSetResult> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionSetId)
                .HasColumnName("question_set_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionSetSubmissionId)
                .HasColumnName("question_set_submission_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.TotalMark)
                .HasColumnName("total_mark")
                .HasColumnType("decimal(20,4)");
            builder
                .Property(x => x.NegativeMark)
                .HasColumnName("negative_mark")
                .HasColumnType("decimal(20,4)");

            builder.ConfigureAuditFields();
        }
    }
}
