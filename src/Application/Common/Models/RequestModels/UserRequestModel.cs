namespace Lingtren.Application.Common.Models.RequestModels
{
    using Lingtren.Domain.Enums;

    public class UserRequestModel
    {
        /// <summary>
        /// Get or set id
        /// </summary>
        public long Id { get; set; }

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
        /// Get or set phone number
        /// </summary>
        public string PhoneNumber { get; set; }

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
        /// Get or set public urls
        /// </summary>
        public string PublicUrls { get; set; }

        /// <summary>
        /// Get or set is active
        /// </summary>
        public bool IsActive { get; set; }

    }
}
