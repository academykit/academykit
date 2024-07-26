namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SkillsConfiguration : IEntityTypeConfiguration<Skills>
    {
        public void Configure(EntityTypeBuilder<Skills> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired(true);

            builder
                .Property(x => x.SkillName)
                .HasColumnName("name")
                .HasColumnType("VARCHAR(250)")
                .HasMaxLength(250)
                .IsRequired(true);
            builder
                .Property(x => x.Description)
                .HasColumnName("remarks")
                .HasColumnType("VARCHAR(250)")
                .HasMaxLength(250)
                .IsRequired(false);
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            builder
                .Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasColumnType("VARCHAR(50)")
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
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired(false);
            builder
                .Property(x => x.UpdatedOn)
                .HasColumnName("updated_on")
                .HasColumnType("DATETIME")
                .IsRequired(false);
            builder
                .HasMany(e => e.EligibilityCreations)
                .WithOne(e => e.Skills)
                .HasForeignKey(e => e.SkillId)
                .IsRequired(false);
            builder
                .HasMany(e => e.SkillsCriteria)
                .WithOne(e => e.SkillType)
                .HasForeignKey(e => e.SkillId)
                .IsRequired(false);
        }
    }
}
