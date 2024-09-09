using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class AssignmentSubmissionAttachmentConfiguration
    : IEntityTypeConfiguration<AssignmentSubmissionAttachment>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmissionAttachment> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.AssignmentSubmissionId)
            .HasColumnName("assignment_submission_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired();
        builder
            .Property(x => x.FileUrl)
            .HasColumnName("file_url")
            .HasColumnType(MigrationConstants.Varchar500)
            .HasMaxLength(500)
            .IsRequired();
        builder
            .Property(x => x.MimeType)
            .HasColumnName("mime_type")
            .HasColumnType(MigrationConstants.Varchar50)
            .IsRequired(false);

        builder.ConfigureAuditFields();
    }
}
