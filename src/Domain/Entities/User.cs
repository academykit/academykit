namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class User : AuditableEntity
    {

        /// <summary>
        /// Get or set first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Get or set middle name
        /// </summary>
        public string MiddleName { get; set; }
        
        /// <summary>
        /// Get or set last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Get or set email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Get or set mobile number
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Get or set role
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Get or set profession
        /// </summary>
        public string Profession { get; set; }

        /// <summary>
        /// Get or set address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Get or set bio
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Get or set hash password
        /// </summary>
        public string HashPassword { get; set; }

        /// <summary>
        /// Get or set public urls
        /// </summary>
        public string PublicUrls { get; set; }

        /// <summary>
        /// Get or set is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get or set password reset token
        /// </summary>
        public string PasswordResetToken { get; set; }

        /// <summary>
        /// Get or set password reset token expiry
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// <summary>
        /// Get or set password change token
        /// </summary>
        public string PasswordChangeToken { get; set; }

        public IList<RefreshToken> RefreshTokens { get; set; }
    }
}
