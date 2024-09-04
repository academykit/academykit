namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SMTPSettingConfiguration : IEntityTypeConfiguration<SMTPSetting>
    {
        public void Configure(EntityTypeBuilder<SMTPSetting> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.MailServer)
                .HasColumnName("mail_server")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(x => x.MailPort).HasColumnName("mail_port").IsRequired();
            builder
                .Property(x => x.SenderName)
                .HasColumnName("sender_name")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();
            builder
                .Property(x => x.SenderEmail)
                .HasColumnName("sender_email")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();
            builder
                .Property(x => x.UserName)
                .HasColumnName("user_name")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.Password)
                .HasColumnName("password")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.ReplyTo)
                .HasColumnName("reply_to")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired();

            builder.ConfigureAuditFields();
        }
    }
}
