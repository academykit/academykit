namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class FeedbackQuestionOptionConfiguration
        : IEntityTypeConfiguration<FeedbackQuestionOption>
    {
        public void Configure(EntityTypeBuilder<FeedbackQuestionOption> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.FeedbackId)
                .HasColumnName("feedback_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Option)
                .HasColumnName("option")
                .HasColumnType("VARCHAR(5000)")
                .HasMaxLength(5000)
                .IsRequired();
            builder.Property(x => x.Order).HasColumnName("order");

            builder.ConfigureAuditFields();
        }
    }
}
