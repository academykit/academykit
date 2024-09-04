﻿namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AssignmentSubmissionAttachmentConfiguration
        : IEntityTypeConfiguration<AssignmentSubmissionAttachment>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmissionAttachment> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.AssignmentSubmissionId)
                .HasColumnName("assignment_submission_id")
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(250)")
                .HasMaxLength(250)
                .IsRequired();
            builder
                .Property(x => x.FileUrl)
                .HasColumnName("file_url")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired();
            builder
                .Property(x => x.MimeType)
                .HasColumnName("mime_type")
                .HasColumnType("varchar(50)")
                .IsRequired(false);
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
