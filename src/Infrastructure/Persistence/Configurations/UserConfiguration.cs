namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.FirstName).HasColumnName("first_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.MiddleName).HasColumnName("middle_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.LastName).HasColumnName("last_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasColumnName("email").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.MobileNumber).HasColumnName("mobile_number").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Role).HasColumnName("role").HasDefaultValue(UserRole.User);
            builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Profession).HasColumnName("profession").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.Address).HasColumnName("address").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Bio).HasColumnName("bio").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.HashPassword).HasColumnName("hash_password").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.PublicUrls).HasColumnName("public_urls").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);
            builder.Property(x => x.PasswordResetToken).HasColumnName("password_reset_token").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.PasswordChangeToken).HasColumnName("password_change_token").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.PasswordResetTokenExpiry).HasColumnName("password_reset_token_expiry").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.HasMany(x => x.RefreshTokens).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Groups).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.GroupMembers).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Tags).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Levels).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Courses).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
        }
    }
}