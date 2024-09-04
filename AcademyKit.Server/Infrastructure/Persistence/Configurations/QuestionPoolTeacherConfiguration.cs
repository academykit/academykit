namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionPoolTeacherConfiguration : IEntityTypeConfiguration<QuestionPoolTeacher>
    {
        public void Configure(EntityTypeBuilder<QuestionPoolTeacher> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionPoolId)
                .HasColumnName("question_pool_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Role).HasColumnName("role").HasDefaultValue(PoolRole.Author);

            builder.ConfigureAuditFields();
        }
    }
}
