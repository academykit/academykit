namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AssignmentAttachmentConfiguration : IEntityTypeConfiguration<AssignmentAttachment>
    {
        public void Configure(EntityTypeBuilder<AssignmentAttachment> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.AssignmentId)
                .HasColumnName("assignment_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("VARCHAR(250)")
                .HasMaxLength(250)
                .IsRequired();
            builder
                .Property(x => x.FileUrl)
                .HasColumnName("file_url")
                .HasColumnType("VARCHAR(500)")
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(x => x.Order).HasColumnName("order").HasDefaultValue(0);
            builder
                .Property(x => x.MimeType)
                .HasColumnName("mime_type")
                .HasColumnType("VARCHAR(50)")
                .IsRequired(false);
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
        }
    }
}
