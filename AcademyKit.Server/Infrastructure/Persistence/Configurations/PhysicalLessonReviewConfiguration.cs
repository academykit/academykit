using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PhysicalLessonReviewConfiguration : IEntityTypeConfiguration<PhysicalLessonReview>
    {
        public void Configure(EntityTypeBuilder<PhysicalLessonReview> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.LessonId)
                .HasColumnName("lesson_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.HasAttended)
                .HasColumnName("hasAttended")
                .HasDefaultValue(false);
            builder.Property(x => x.IsReviewed).HasColumnName("is_reviewed").HasDefaultValue(false);
            builder
                .Property(x => x.ReviewMessage)
                .HasColumnName("review_message")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder.ConfigureAuditFields();
        }
    }
}
