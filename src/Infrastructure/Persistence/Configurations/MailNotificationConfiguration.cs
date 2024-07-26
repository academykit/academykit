namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MailNotificationConfiguration : IEntityTypeConfiguration<MailNotification>
    {
        public void Configure(EntityTypeBuilder<MailNotification> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(x => x.Name)
                .HasColumnName("mail_name")
                .HasColumnType("VARCHAR(250)")
                .HasMaxLength(250)
                .IsRequired(true);

            builder
                .Property(x => x.Subject)
                .HasColumnName("mail_subject")
                .HasColumnType("VARCHAR(500)")
                .HasMaxLength(500)
                .IsRequired(true);
            builder
                .Property(x => x.Message)
                .HasColumnName("mail_message")
                .HasColumnType("TEXT")
                .IsRequired(true);

            builder.Property(x => x.MailType).HasColumnName("mail_type").IsRequired(true);

            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);

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
