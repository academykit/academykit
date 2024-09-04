namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.LessonId)
                .HasColumnName("lesson_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.AssignmentId)
                .HasColumnName("assignment_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.IsCorrect).HasColumnName("is_correct").HasDefaultValue(false);
            builder
                .Property(x => x.SelectedOption)
                .HasColumnName("selected_option")
                .HasColumnType("varchar(300)")
                .HasMaxLength(300)
                .IsRequired(false);
            builder
                .Property(x => x.Answer)
                .HasColumnName("answer")
                .HasColumnType("varchar(5000)")
                .HasMaxLength(5000)
                .IsRequired(false);
            builder
                .Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.CreatedOn)
                .HasColumnName("created_on")
                .IsRequired()
                .HasColumnType("DATETIME");
            builder
                .Property(x => x.UpdatedBy)
                .HasColumnName("updated_by")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME")
                .IsRequired(false);
            builder
                .HasMany(x => x.AssignmentSubmissionAttachments)
                .WithOne(x => x.AssignmentSubmission)
                .HasForeignKey(x => x.AssignmentSubmissionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
