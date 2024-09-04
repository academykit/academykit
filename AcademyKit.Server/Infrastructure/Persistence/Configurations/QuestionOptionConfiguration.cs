namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
    {
        public void Configure(EntityTypeBuilder<QuestionOption> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.QuestionId)
                .HasColumnName("question_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Option).HasColumnName("option").HasMaxLength(5000).IsRequired();
            builder.Property(x => x.Order).HasColumnName("order");
            builder
                .Property(x => x.IsCorrect)
                .HasColumnName("is_correct")
                .HasDefaultValue(0)
                .IsRequired();

            builder.ConfigureAuditFields();
        }
    }
}
