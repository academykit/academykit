namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;

    public class QuestionSetConfiguration : IEntityTypeConfiguration<QuestionSet>
    {
        public void Configure(EntityTypeBuilder<QuestionSet> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(500).IsRequired();
            builder.Property(x => x.Slug).HasColumnName("slug").IsRequired();
            builder.Property(x => x.ThumbnailUrl).HasColumnName("thumbnail_url").IsRequired(false);
            builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(5000).IsRequired(false);
            builder.Property(x => x.TotalQuestion).HasColumnName("total_question");
            builder.Property(x => x.NegativeMarking).HasColumnName("negative_marking").HasDefaultValue(0).HasColumnType("decimal(10,4)");
            builder.Property(x => x.QuestionMarking).HasColumnName("question_marking").IsRequired().HasColumnType("decimal(10,4)");
            builder.Property(x => x.QuestionSetType).HasColumnName("question_set_type");
            builder.Property(x => x.AllowedRetake).HasColumnName("allowed_retake").HasDefaultValue(1);
            builder.Property(x => x.Duration).HasColumnName("duration");
            builder.Property(x => x.StartTime).HasColumnName("start_time").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.EndTime).HasColumnName("end_time").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(0);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.HasMany(x => x.QuestionSetQuestions).WithOne(x => x.QuestionSet).HasForeignKey(x => x.QuestionSetId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}