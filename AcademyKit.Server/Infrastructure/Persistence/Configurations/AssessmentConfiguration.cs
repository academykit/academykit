using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Slug)
            .HasColumnName("slug")
            .HasColumnType("VARCHAR(270)")
            .HasMaxLength(270)
            .IsRequired();
        builder
            .Property(x => x.Title)
            .HasColumnName("title")
            .HasColumnType("VARCHAR(250)")
            .HasMaxLength(250)
            .IsRequired();
        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder
            .Property(x => x.Message)
            .HasColumnName("message")
            .HasColumnType("VARCHAR(500)")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(x => x.AssessmentStatus).HasColumnName("assessment_status");

        builder.Property(x => x.Retakes).HasColumnName("retake").HasDefaultValue(0);
        builder.Property(x => x.Duration).HasColumnName("duration").HasDefaultValue(0);
        builder.Property(x => x.Weightage).HasColumnName("weightage").HasDefaultValue(0);
        builder
            .Property(x => x.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("DATETIME")
            .IsRequired(true);
        builder
            .Property(x => x.EndDate)
            .HasColumnName("end_date")
            .HasColumnType("DATETIME")
            .IsRequired(true);

        builder.ConfigureAuditFields();

        builder
            .HasMany(e => e.SkillsCriteria)
            .WithOne(e => e.Assessment)
            .HasForeignKey(e => e.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder
            .HasMany(e => e.AssessmentQuestions)
            .WithOne(e => e.Assessment)
            .HasForeignKey(e => e.AssessmentId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
        builder
            .HasMany(x => x.AssessmentSubmissions)
            .WithOne(x => x.Assessment)
            .HasForeignKey(x => x.AssessmentId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(e => e.EligibilityCreations)
            .WithOne(e => e.Assessment)
            .HasForeignKey(e => e.AssessmentId)
            .IsRequired(false);

        builder
            .HasMany(e => e.EligibilityCreationsCompleted)
            .WithOne(e => e.CompletedAssessment)
            .HasForeignKey(e => e.CompletedAssessmentId)
            .IsRequired(false);
        builder
            .HasMany(x => x.AssessmentResults)
            .WithOne(x => x.Assessment)
            .HasForeignKey(x => x.AssessmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
