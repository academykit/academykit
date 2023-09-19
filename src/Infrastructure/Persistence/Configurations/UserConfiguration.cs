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
            #region Basic
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Role).HasColumnName("role").HasDefaultValue(UserRole.Trainee);
            builder.Property(x => x.HashPassword).HasColumnName("hash_password").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.PublicUrls).HasColumnName("public_urls").HasColumnType("VARCHAR(2000)").HasMaxLength(2000).IsRequired(false);
            builder.Property(x => x.Status).HasColumnName("status").HasDefaultValue(UserStatus.Active).IsRequired();
            builder.Property(x => x.PasswordResetToken).HasColumnName("password_reset_token").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.PasswordChangeToken).HasColumnName("password_change_token").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.PasswordResetTokenExpiry).HasColumnName("password_reset_token_expiry").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.FirstName).HasColumnName("first_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.MiddleName).HasColumnName("middle_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.LastName).HasColumnName("last_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
            builder.Property(x => x.Gender).HasColumnName("gender");
            builder.Property(x => x.BirthDateBS).HasColumnName("birth_date_bs").IsRequired(false).HasColumnType("DATETIME");
            builder.Property(x => x.BirthDateAD).HasColumnName("birth_date_ad").IsRequired(false).HasColumnType("DATETIME");
            builder.Property(x => x.MaritalStatus).HasColumnName("marital_status");
            builder.Property(x => x.BloodGroup).HasColumnName("blood_group");
            builder.Property(x => x.Nationality).HasColumnName("nationality").HasDefaultValue(Nationality.Nepali);
            #endregion
            #region Official Info
            builder.Property(x => x.MemberId).HasColumnName("member_id").HasColumnType("VARCHAR(50)").IsRequired(false);
            builder.Property(x => x.EmploymentType).HasColumnName("employment_type").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.BranchId).HasColumnName("branch_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Profession).HasColumnName("profession").HasColumnType("VARCHAR(250)").HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.DepartmentId).HasColumnName("department_id").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.JoinedDateBS).HasColumnName("joined_date_bs").IsRequired(false).HasColumnType("DATETIME");
            builder.Property(x => x.JoinedDateAD).HasColumnName("joined_date_ad").IsRequired(false).HasColumnType("DATETIME");
            #endregion
            #region Address
            #region Permanent Address
            builder.Property(x => x.Address).HasColumnName("address").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.PermanentCountry).HasColumnName("permanent_country").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.PermanentState).HasColumnName("permanent_state").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.PermanentCity).HasColumnName("permanent_city").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.PermanentMunicipality).HasColumnName("permanent_municipality").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.PermanentWard).HasColumnName("permanent_ward").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.PermanentDistrict).HasColumnName("permanent_district").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            #endregion
            #region Current Address
            builder.Property(x => x.CurrentCountry).HasColumnName("current_country").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CurrentState).HasColumnName("current_state").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CurrentCity).HasColumnName("current_city").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CurrentMunicipality).HasColumnName("current_municipality").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CurrentWard).HasColumnName("current_ward").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CurrentAddress).HasColumnName("current_address").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CurrentDistrict).HasColumnName("current_district").HasColumnType("VARCHAR(200)").HasMaxLength(200).IsRequired(false);
            #endregion
            #endregion
            #region Contact Details
            builder.Property(x => x.Email).HasColumnName("email").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(); // Login email (Office Email)
            builder.Property(x => x.MobileNumber).HasColumnName("mobile_number").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false); // Primary Mobile number
            builder.Property(x => x.PersonalEmail).HasColumnName("personal_email").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.MobileNumberSecondary).HasColumnName("mobile_number_secondary").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            #endregion
            #region Identification Details
            builder.Property(x => x.IdentityType).HasColumnName("identity_type");
            builder.Property(x => x.IdentityNumber).HasColumnName("identity_number").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.IdentityIssuedBy).HasColumnName("identity_issued_by").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.IdentityIssuedOn).HasColumnName("identity_issued_on").HasColumnType("DATE").HasMaxLength(100).IsRequired(false);
            #endregion
            #region Family details
            builder.Property(x => x.FatherName).HasColumnName("father_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.MotherName).HasColumnName("mother_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.SpouseName).HasColumnName("spouse_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.GrandFatherName).HasColumnName("grandfather_name").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.MemberPhone).HasColumnName("member_phone").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.MemberPermanentAddress).HasColumnName("member_permanent_address").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.MemberCurrentAddress).HasColumnName("member_current_address").HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.FamilyAddressIsSame).HasColumnName("family_address_same");
            #endregion
            builder.Property(x => x.Bio).HasColumnName("bio").HasColumnType("VARCHAR(500)").HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedOn).HasColumnName("updated_on").HasColumnType("DATETIME").IsRequired(false);
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("VARCHAR(50)").HasMaxLength(50).IsRequired(false);

            // foreign relationships configuration
            builder.HasMany(x => x.RefreshTokens).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Groups).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.GroupMembers).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Tags).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Levels).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Courses).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Sections).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Lessons).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseTeachers).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.ZoomSettings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.ZoomLicenses).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.SMTPSettings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Meetings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.MeetingReports).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.GeneralSettings).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Departments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseTags).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSets).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionPools).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Questions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionOptions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionPoolQuestions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetQuestions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionPoolTeachers).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetResults).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetSubmissions).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionSetSubmissionAnswers).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseEnrollments).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.WatchHistories).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Assignments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentAttachments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentSubmissions).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.QuestionTags).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentQuestionOptions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentSubmissionAttachments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Comments).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CommentReplies).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.GroupStorages).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.AssignmentReviews).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Feedbacks).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.FeedbackQuestionOptions).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.FeedbackSubmissions).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Signatures).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CourseCertificates).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Certificates).WithOne(x => x.User).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction);
        }
    }
}