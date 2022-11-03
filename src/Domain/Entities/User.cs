namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class User : AuditableEntity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public UserRole Role { get; set; }
        public string Profession { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public string HashPassword { get; set; }
        public string PublicUrls { get; set; }
        public bool IsActive { get; set; }
        public string PasswordResetToken { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string PasswordChangeToken { get; set; }
        public IList<RefreshToken> RefreshTokens { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<GroupMember> GroupMembers { get; set; }
        public IList<Tag> Tags { get; set; }
        public IList<Level> Levels { get; set; }
        public IList<Course> Courses { get; set; }
    }
}
