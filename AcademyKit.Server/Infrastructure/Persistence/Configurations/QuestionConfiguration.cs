using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired();
        builder
            .Property(x => x.Type)
            .HasColumnName("type")
            .HasDefaultValue(QuestionTypeEnum.MultipleChoice);
        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(MigrationConstants.Varchar5000)
            .HasMaxLength(5000)
            .IsRequired(false);
        builder
            .Property(x => x.Hints)
            .HasColumnName("hints")
            .HasColumnType(MigrationConstants.Varchar5000)
            .HasMaxLength(5000)
            .IsRequired(false);

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.QuestionOptions)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionPoolQuestions)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetQuestions)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
