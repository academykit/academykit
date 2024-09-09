using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class QuestionPoolConfiguration : IEntityTypeConfiguration<QuestionPool>
{
    public void Configure(EntityTypeBuilder<QuestionPool> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.Slug)
            .HasColumnName("slug")
            .HasColumnType(MigrationConstants.Varchar105)
            .HasMaxLength(105)
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
