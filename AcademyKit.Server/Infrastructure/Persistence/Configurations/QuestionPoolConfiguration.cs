namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionPoolConfiguration : IEntityTypeConfiguration<QuestionPool>
    {
        public void Configure(EntityTypeBuilder<QuestionPool> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.Slug)
                .HasColumnName("slug")
                .HasColumnType("VARCHAR(105)")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.QuestionPoolQuestions)
                .WithOne(x => x.QuestionPool)
                .HasForeignKey(x => x.QuestionPoolId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
