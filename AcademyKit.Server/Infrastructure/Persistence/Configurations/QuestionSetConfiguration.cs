using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class QuestionSetConfiguration : IEntityTypeConfiguration<QuestionSet>
{
    public void Configure(EntityTypeBuilder<QuestionSet> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired();
        builder
            .Property(x => x.Slug)
            .HasColumnName("slug")
            .HasColumnType(MigrationConstants.Varchar520)
            .HasMaxLength(520)
            .IsRequired();
        builder
            .Property(x => x.ThumbnailUrl)
            .HasColumnName("thumbnail_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired(false);
        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(MigrationConstants.Varchar5000)
            .HasMaxLength(5000)
            .IsRequired(false);
        builder
            .Property(x => x.NegativeMarking)
            .HasColumnName("negative_marking")
            .HasColumnType(MigrationConstants.Decimal10_4)
            .HasDefaultValue(0);
        builder
            .Property(x => x.QuestionMarking)
            .HasColumnName("question_marking")
            .HasColumnType(MigrationConstants.Decimal10_4)
            .IsRequired();
        builder
            .Property(x => x.PassingWeightage)
            .HasColumnName("passing_weightage")
            .HasColumnType(MigrationConstants.Decimal10_4);
        builder.Property(x => x.AllowedRetake).HasColumnName("allowed_retake").HasDefaultValue(1);
        builder.Property(x => x.Duration).HasColumnName("duration");
        builder
            .Property(x => x.StartTime)
            .HasColumnName("start_time")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(false);
        builder
            .Property(x => x.EndTime)
            .HasColumnName("end_time")
            .HasColumnType(MigrationConstants.DateTime)
            .IsRequired(false);
        builder.Property(x => x.NoOfQuestion).HasColumnName("no_of_question");
        builder.Property(x => x.ShowAll).HasColumnName("show_all").HasDefaultValue(true);
        builder.Property(x => x.IsShuffle).HasColumnName("is_shuffle").HasDefaultValue(false);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(0);

        builder.ConfigureAuditFields();

        builder
            .HasMany(x => x.QuestionSetQuestions)
            .WithOne(x => x.QuestionSet)
            .HasForeignKey(x => x.QuestionSetId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne(x => x.Lesson)
            .WithOne(x => x.QuestionSet)
            .HasForeignKey<Lesson>(x => x.QuestionSetId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetResults)
            .WithOne(x => x.QuestionSet)
            .HasForeignKey(x => x.QuestionSetId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(x => x.QuestionSetSubmissions)
            .WithOne(x => x.QuestionSet)
            .HasForeignKey(x => x.QuestionSetId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
