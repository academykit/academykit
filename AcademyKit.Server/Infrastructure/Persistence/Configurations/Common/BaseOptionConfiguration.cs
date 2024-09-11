using AcademyKit.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations.Common;

public static class OptionConfigurationExtensions
{
    public static void ConfigureCommonOptionProperties<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        builder
            .Property("Option")
            .HasColumnName("option")
            .HasColumnType(MigrationConstants.Varchar5000)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property("Order").HasColumnName("order");
    }

    public static void ConfigureCorrectOption<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        builder.Property("IsCorrect").HasColumnName("is_correct").HasDefaultValue(0).IsRequired();
    }
}
