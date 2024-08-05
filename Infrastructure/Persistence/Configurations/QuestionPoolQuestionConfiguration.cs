﻿namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionPoolQuestionConfiguration : IEntityTypeConfiguration<QuestionPoolQuestion>
    {
        public void Configure(EntityTypeBuilder<QuestionPoolQuestion> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionId)
                .HasColumnName("question_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.QuestionPoolId)
                .HasColumnName("question_pool_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("VARCHAR(50)")
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
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME")
                .IsRequired(false);
            builder
                .HasMany(x => x.QuestionSetQuestions)
                .WithOne(x => x.QuestionPoolQuestion)
                .HasForeignKey(x => x.QuestionPoolQuestionId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(x => x.Order).HasColumnName("order").HasDefaultValue(0);
        }
    }
}
