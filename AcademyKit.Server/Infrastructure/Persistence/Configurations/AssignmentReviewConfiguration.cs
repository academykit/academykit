using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssignmentReviewConfiguration : IEntityTypeConfiguration<AssignmentReview>
{
    public void Configure(EntityTypeBuilder<AssignmentReview> builder)
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
        builder
            .Property(x => x.Mark)
            .HasColumnName("mark")
            .IsRequired()
            .HasColumnType(MigrationConstants.Decimal20_4);
        builder.Property(x => x.Review).HasColumnName("review").HasMaxLength(500).IsRequired(false);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.ConfigureAuditFields();
    }
}
