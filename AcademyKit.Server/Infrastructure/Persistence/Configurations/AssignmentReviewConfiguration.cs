namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AssignmentReviewConfiguration : IEntityTypeConfiguration<AssignmentReview>
    {
        public void Configure(EntityTypeBuilder<AssignmentReview> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.LessonId)
                .HasColumnName("lesson_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Mark)
                .HasColumnName("mark")
                .IsRequired()
                .HasColumnType("decimal(20,4)");
            builder
                .Property(x => x.Review)
                .HasColumnName("review")
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

            builder.ConfigureAuditFields();
        }
    }
}
