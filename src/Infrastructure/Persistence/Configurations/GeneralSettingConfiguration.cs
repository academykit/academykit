namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class GeneralSettingConfiguration : IEntityTypeConfiguration<GeneralSetting>
    {
        public void Configure(EntityTypeBuilder<GeneralSetting> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.LogoUrl).HasColumnName("logo_url").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired();
            builder.Property(x => x.CompanyName).HasColumnName("company_name").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.CompanyAddress).HasColumnName("company_address").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.CompanyContactNumber).HasColumnName("company_contact_number").HasColumnType("VARCHAR(30)").HasMaxLength(30).IsRequired();
            builder.Property(x => x.EmailSignature).HasColumnName("email_signature").HasColumnType("VARCHAR(1000)").HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x=>x.CustomConfiguration).HasColumnName("custom_configuration").HasColumnType("VARCHAR(5000)").IsRequired(false);
        }
    }
}