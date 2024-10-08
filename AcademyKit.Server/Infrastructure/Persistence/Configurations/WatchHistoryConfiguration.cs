﻿using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence.Configurations.Common;
using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class WatchHistoryConfiguration : IEntityTypeConfiguration<WatchHistory>
{
    public void Configure(EntityTypeBuilder<WatchHistory> builder)
    {
        builder.ConfigureId();

        builder
            .Property(x => x.CourseId)
            .HasColumnName("course_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.LessonId)
            .HasColumnName("lesson_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.IsCompleted).HasColumnName("is_completed").HasDefaultValue(0);
        builder.Property(x => x.IsPassed).HasColumnName("is_passed").HasDefaultValue(0);
        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();

        builder.ConfigureAuditFields();
    }
}
