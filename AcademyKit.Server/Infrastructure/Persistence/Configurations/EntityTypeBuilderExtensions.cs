using AcademyKit.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public static class EntityTypeBuilderExtensions
{
    public static void ConfigureId<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : IdentifiableEntity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasColumnType("varchar(50)").IsRequired();
    }

    public static void ConfigureAuditFields<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity
    {
        builder
            .Property(e => e.CreatedBy)
            .HasColumnName("created_by")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(e => e.CreatedOn)
            .HasColumnName("created_on")
            .HasColumnType("datetime")
            .IsRequired();

        builder
            .Property(e => e.UpdatedBy)
            .HasColumnName("updated_by")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired(false);

        builder
            .Property(e => e.UpdatedOn)
            .HasColumnName("updated_on")
            .HasColumnType("datetime")
            .IsRequired(false);
    }
}
