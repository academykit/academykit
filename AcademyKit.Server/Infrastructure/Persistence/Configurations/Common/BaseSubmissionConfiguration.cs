using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations.Common;

public static class BaseSubmissionConfiguration
{
    public static void ConfigureCommonSubmissionProperties<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        builder
            .Property("UserId")
            .HasColumnName("user_id")
            .HasColumnType(MigrationConstants.Varchar50)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property("StartTime")
            .HasColumnName("start_time")
            .HasColumnType(MigrationConstants.DateTime);
        builder
            .Property("EndTime")
            .HasColumnName("end_time")
            .HasColumnType(MigrationConstants.DateTime);
        builder
            .Property("IsSubmissionError")
            .HasColumnName("is_submission_error")
            .HasDefaultValue(false);
        builder
            .Property("SubmissionErrorMessage")
            .HasColumnName("submission_error_message")
            .HasColumnType(MigrationConstants.Varchar250)
            .HasMaxLength(250)
            .IsRequired(false);
    }
}
