using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class QuestionSetQuestionConfiguration : IEntityTypeConfiguration<QuestionSetQuestion>
{
    public void Configure(EntityTypeBuilder<QuestionSetQuestion> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.Order).HasColumnName("order");
        builder
            .Property(x => x.QuestionSetId)
            .HasColumnName("question_set_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.QuestionId)
            .HasColumnName("question_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);
        builder
            .Property(x => x.QuestionPoolQuestionId)
            .HasColumnName("question_pool_question_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
