namespace Lingtren.Infrastructure.Persistence.Configurations
{
    using Lingtren.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class UserEducationConfiguration : IEntityTypeConfiguration<UserEducation>
    {
        public void Configure(EntityTypeBuilder<UserEducation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.UserId).HasColumnName("user_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.InstitutionName).HasColumnName("institution_name").HasColumnType("VARCHAR(270)").HasMaxLength(270).IsRequired();
            builder.Property(x => x.Degree).HasColumnName("degree").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Specialization).HasColumnName("specialization").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired();
            builder.Property(x => x.CompletionDate).HasColumnName("completion_date").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.HasOne(x => x.User).WithMany(x => x.UserEducations).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
