using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyKit.Infrastructure.Persistence.Configurations;

public class UserSkillsConfiguration : IEntityTypeConfiguration<UserSkills>
{
    public void Configure(EntityTypeBuilder<UserSkills> builder)
    {
        builder.HasKey(us => new { us.SkillId, us.UserId });
        builder.HasOne(us => us.User).WithMany(us => us.UserSkills).HasForeignKey(us => us.UserId);

        builder
            .HasOne(us => us.Skills)
            .WithMany(us => us.UserSkills)
            .HasForeignKey(us => us.SkillId);
    }
}
