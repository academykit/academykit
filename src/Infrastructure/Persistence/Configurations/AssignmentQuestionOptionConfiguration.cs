namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Lingtren.Domain.Entities;

    public class AssignmentQuestionOptionConfiguration : IEntityTypeConfiguration<AssignmentQuestionOption>
    {
        public void Configure(EntityTypeBuilder<AssignmentQuestionOption> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.AssignmentId).HasColumnName("assignment_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Option).HasColumnName("option").HasColumnType("VARCHAR(5000)").HasMaxLength(5000).IsRequired();
            builder.Property(x => x.Order).HasColumnName("order");
            builder.Property(x => x.IsCorrect).HasColumnName("is_correct").HasDefaultValue(0);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
        }
    }
}