namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CommentRepliesConfiguration : IEntityTypeConfiguration<CommentReply>
    {
        public void Configure(EntityTypeBuilder<CommentReply> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.CommentId)
                .HasColumnName("comment_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Content)
                .HasColumnName("content")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            builder
                .Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("varchar(50)")
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
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME")
                .IsRequired(false);
        }
    }
}
