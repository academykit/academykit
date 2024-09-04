namespace AcademyKit.Infrastructure.Persistence.Configurations
{
    using AcademyKit.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ConfigureId();

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType("VARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(x => x.Token)
                .HasColumnName("token")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired(false);
            builder.Property(x => x.LoginAt).HasColumnName("login_at").HasColumnType("DATETIME");
            builder
                .Property(x => x.DeviceId)
                .HasColumnName("device_id")
                .HasColumnType("VARCHAR(200)")
                .HasMaxLength(200)
                .IsRequired(false);
            builder
                .Property(x => x.Location)
                .HasColumnName("location")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100)
                .IsRequired(false);
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);

            builder.ConfigureAuditFields();
        }
    }
}
