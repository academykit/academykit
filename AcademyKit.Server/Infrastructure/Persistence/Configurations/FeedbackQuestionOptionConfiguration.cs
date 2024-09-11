using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class FeedbackQuestionOptionConfiguration : IEntityTypeConfiguration<FeedbackQuestionOption>
{
    public void Configure(EntityTypeBuilder<FeedbackQuestionOption> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.FeedbackId)
            .HasColumnName("feedback_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.ConfigureCommonOptionProperties();

        builder.ConfigureAuditFields();
    }
}
