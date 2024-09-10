using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class SMTPSettingConfiguration : IEntityTypeConfiguration<SMTPSetting>
{
    public void Configure(EntityTypeBuilder<SMTPSetting> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.MailServer)
            .HasColumnName("mail_server")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.MailPort).HasColumnName("mail_port").IsRequired();
        builder
            .Property(x => x.SenderName)
            .HasColumnName("sender_name")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();
        builder
            .Property(x => x.SenderEmail)
            .HasColumnName("sender_email")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();
        builder
            .Property(x => x.UserName)
            .HasColumnName("user_name")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.Password)
            .HasColumnName("password")
            .HasColumnType(MigrationConstants.Varchar100)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.ReplyTo)
            .HasColumnName("reply_to")
            .HasColumnType(MigrationConstants.Varchar200)
            .HasMaxLength(200)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
