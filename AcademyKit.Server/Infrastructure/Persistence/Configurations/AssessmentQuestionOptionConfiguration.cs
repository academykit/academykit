using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssessmentQuestionOptionConfiguration : IEntityTypeConfiguration<AssessmentOptions>
{
    public void Configure(EntityTypeBuilder<AssessmentOptions> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.AssessmentQuestionId)
            .HasColumnName("assessment_question_id")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(x => x.Option)
            .HasColumnName("option")
            .HasColumnType("varchar(5000)")
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(x => x.Order).HasColumnName("order");

        builder
            .Property(x => x.IsCorrect)
            .HasColumnName("is_correct")
            .HasDefaultValue(0)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
