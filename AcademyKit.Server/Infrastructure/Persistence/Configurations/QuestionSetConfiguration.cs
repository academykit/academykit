namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuestionSetConfiguration : IEntityTypeConfiguration<QuestionSet>
    {
        public void Configure(EntityTypeBuilder<QuestionSet> builder)
        {
            builder.ConfigureId();

            builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(500).IsRequired();
            builder
                .Property(x => x.Slug)
                .HasColumnName("slug")
                .HasColumnType("varchar(520)")
                .HasMaxLength(520)
                .IsRequired();
            builder
                .Property(x => x.ThumbnailUrl)
                .HasColumnName("thumbnail_url")
                .HasMaxLength(500)
                .IsRequired(false);
            builder
                .Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(5000)
                .IsRequired(false);
            builder
                .Property(x => x.NegativeMarking)
                .HasColumnName("negative_marking")
                .HasDefaultValue(0)
                .HasColumnType("decimal(10,4)");
            builder
                .Property(x => x.QuestionMarking)
                .HasColumnName("question_marking")
                .IsRequired()
                .HasColumnType("decimal(10,4)");
            builder
                .Property(x => x.PassingWeightage)
                .HasColumnName("passing_weightage")
                .HasColumnType("decimal(10,4)");
            builder
                .Property(x => x.AllowedRetake)
                .HasColumnName("allowed_retake")
                .HasDefaultValue(1);
            builder.Property(x => x.Duration).HasColumnName("duration");
            builder
                .Property(x => x.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("DATETIME")
                .IsRequired(false);
            builder
                .Property(x => x.EndTime)
                .HasColumnName("end_time")
                .HasColumnType("DATETIME")
                .IsRequired(false);
            builder.Property(x => x.NoOfQuestion).HasColumnName("no_of_question");
            builder.Property(x => x.ShowAll).HasColumnName("show_all").HasDefaultValue(true);
            builder.Property(x => x.IsShuffle).HasColumnName("is_shuffle").HasDefaultValue(false);
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(0);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.QuestionSetQuestions)
                .WithOne(x => x.QuestionSet)
                .HasForeignKey(x => x.QuestionSetId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(x => x.Lesson)
                .WithOne(x => x.QuestionSet)
                .HasForeignKey<Lesson>(x => x.QuestionSetId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.QuestionSetResults)
                .WithOne(x => x.QuestionSet)
                .HasForeignKey(x => x.QuestionSetId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasMany(x => x.QuestionSetSubmissions)
                .WithOne(x => x.QuestionSet)
                .HasForeignKey(x => x.QuestionSetId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
