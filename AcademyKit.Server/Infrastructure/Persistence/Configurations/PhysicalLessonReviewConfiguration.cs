using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PhysicalLessonReviewConfiguration : IEntityTypeConfiguration<PhysicalLessonReview>
{
    public void Configure(EntityTypeBuilder<PhysicalLessonReview> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.LessonId)
            .HasColumnName("lesson_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.HasAttended).HasColumnName("hasAttended").HasDefaultValue(false);
        builder.Property(x => x.IsReviewed).HasColumnName("is_reviewed").HasDefaultValue(false);
        builder
            .Property(x => x.ReviewMessage)
            .HasColumnName("review_message")
            .HasColumnType(MigrationConstants.Varchar500)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
