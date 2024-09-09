using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class QuestionPoolTeacherConfiguration : IEntityTypeConfiguration<QuestionPoolTeacher>
{
    public void Configure(EntityTypeBuilder<QuestionPoolTeacher> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.QuestionPoolId)
            .HasColumnName("question_pool_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").HasDefaultValue(PoolRole.Author);

        builder.ConfigureAuditFields();
    }
}
