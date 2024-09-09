using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
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
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();

        builder.ConfigureCommonOptionProperties();
        builder.ConfigureCorrectOption();

        builder.ConfigureAuditFields();
    }
}
