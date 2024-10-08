﻿namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Persistence.Configurations.Common;
    using AcademyKit.Infrastructure.Persistence.Migrations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Slug)
                .HasColumnName("slug")
                .HasColumnType(MigrationConstants.Varchar270)
                .HasMaxLength(270)
                .IsRequired();
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType(MigrationConstants.Varchar250)
                .HasMaxLength(250)
                .IsRequired();
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

            builder.ConfigureAuditFields();

            builder
                .HasMany(x => x.CourseTags)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
