namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MailNotificationConfiguration : IEntityTypeConfiguration<MailNotification>
    {
        public void Configure(EntityTypeBuilder<MailNotification> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Name)
                .HasColumnName("mail_name")
                .HasColumnType("varchar(250)")
                .HasMaxLength(250)
                .IsRequired(true);

            builder
                .Property(x => x.Subject)
                .HasColumnName("mail_subject")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired(true);
            builder
                .Property(x => x.Message)
                .HasColumnName("mail_message")
                .HasColumnType("TEXT")
                .IsRequired(true);

            builder.Property(x => x.MailType).HasColumnName("mail_type").IsRequired(true);

            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            builder.ConfigureAuditFields();
        }
    }
}
