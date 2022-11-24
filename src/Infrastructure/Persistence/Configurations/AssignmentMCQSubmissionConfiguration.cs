namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;

    public class AssignmentMCQSubmissionConfiguration : IEntityTypeConfiguration<AssignmentMCQSubmission>
    {
        public void Configure(EntityTypeBuilder<AssignmentMCQSubmission> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.AssignmentId).HasColumnName("assignment_id").HasColumnType("VARCHAR(50)").HasMaxLength(20).IsRequired();
            builder.Property(x => x.IsCorrect).HasColumnName("is_correct").HasDefaultValue(false);
            builder.Property(x => x.SelectedOption).HasColumnName("selected_option").HasColumnType("VARCHAR(300)").HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}