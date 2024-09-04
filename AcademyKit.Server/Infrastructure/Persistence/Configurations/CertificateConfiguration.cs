namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(200)")
                .HasMaxLength(200)
                .IsRequired();
            builder
                .Property(x => x.StartDate)
                .HasColumnName("start_date")
                .HasColumnType("DATETIME");
            builder.Property(x => x.EndDate).HasColumnName("end_date").HasColumnType("DATETIME");
            builder
                .Property(x => x.ImageUrl)
                .HasColumnName("image_url")
                .HasColumnType("varchar(200)")
                .IsRequired(false);
            builder
                .Property(x => x.Location)
                .HasColumnName("location")
                .HasColumnType("varchar(100)")
                .IsRequired(false);
            builder
                .Property(x => x.Institute)
                .HasColumnName("institute")
                .HasColumnType("varchar(100)")
                .IsRequired(false);
            builder.Property(x => x.Duration).HasColumnName("duration").IsRequired(false);
            builder
                .Property(x => x.OptionalCost)
                .HasColumnName("optional_cost")
                .HasColumnType("DECIMAL(10,2)")
                .HasDefaultValue(0);
            builder.Property(x => x.Status).HasColumnName("status");
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
