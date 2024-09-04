namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.LessonId)
                .HasColumnName("lesson_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired(false);
            builder
                .Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(5000)")
                .HasMaxLength(5000)
                .IsRequired(false);
            builder.Property(x => x.Order).HasColumnName("order").HasDefaultValue(0);
            builder
                .Property(x => x.Hints)
                .HasColumnName("hints")
                .HasColumnType("varchar(5000)")
                .HasMaxLength(5000)
                .IsRequired(false);
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);
            builder.Property(x => x.Type).HasColumnName("type").IsRequired();

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.AssignmentAttachments)
                .WithOne(x => x.Assignment)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.AssignmentSubmissions)
                .WithOne(x => x.Assignment)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.AssignmentQuestionOptions)
                .WithOne(x => x.Assignment)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
