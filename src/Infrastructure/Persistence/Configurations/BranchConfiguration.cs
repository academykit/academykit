namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Slug).HasColumnName("slug").HasColumnType("VARCHAR(270)").HasMaxLength(270).IsRequired();
            builder.Property(x => x.BranchCode).HasColumnName("code").HasColumnType("VARCHAR(270)").HasMaxLength(270).IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.NameNepali).HasColumnName("nepaliname").HasColumnType("VARCHAR(270)").HasMaxLength(270).IsRequired(false);
            builder.Property(x => x.Address).HasColumnName("address").HasColumnType("VARCHAR(270)").HasMaxLength(270).IsRequired();
            builder.Property(x => x.Email).HasColumnName("email").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.MobileNumber).HasColumnName("mobile_number").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.SolId).HasColumnName("sol_id").HasColumnType("INT(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Remarks).HasColumnName("remarks").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            // builder.Property(x => x.BranchHead).HasColumnName("branch_head").IsRequired(false);
            // builder.Property(x => x.OperationIncharge).HasColumnName("operation_incharge").IsRequired(false);
            builder.Property(x => x.Location).HasColumnName("location").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.UnderBranch).HasColumnName("under_branch").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.AreaBranch).HasColumnName("area_branch").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.RegionalBranch).HasColumnName("regional_branch").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Province).HasColumnName("province").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.ValleyType).HasColumnName("valley_type").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            // builder.HasMany(x => x.Users).WithOne(x => x.BranchName).HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
