using Ganss.Xss;
using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class UserResponseModel
    {
        /// <summary>
        /// Get or set id
        /// </summary>
        public Guid Id { get; set; }

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
        /// Get or set public urls
        /// </summary>
        public string PublicUrls { get; set; }

        /// <summary>
        /// Get or set is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get or set created on
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Get or set FullName
        /// </summary>
        public string FullName { get; set; }
        public UserResponseModel()
        {

        }
        public UserResponseModel(User user)
        {
            var sanitizer = new HtmlSanitizer
            {
                KeepChildNodes = true,
            };
            Id = user.Id;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            LastName = user.LastName;
            Email = user.Email;
            MobileNumber = user.MobileNumber;
            Role = user.Role;
            Profession = user.Profession;
            Address = user.Address;
            Bio = user.Bio;
            PublicUrls = user.PublicUrls;
            IsActive = user.IsActive;
            CreatedOn = user.CreatedOn;
            FullName = sanitizer.Sanitize($"{user.FirstName} {user.MiddleName} {user.LastName}");
        }
    }
}
