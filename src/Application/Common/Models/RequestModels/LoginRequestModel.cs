using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public class LoginRequestModel : IValidatableObject
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public bool IsEmail { get; private set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            //Validate email format
            const string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                   @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                      @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            if (new Regex(emailRegex).IsMatch(Email))
            {
                IsEmail = true;
            }
            else
            {
                result.Add(new ValidationResult("Invalid Email", new[] { nameof(Email) }));
            }

            return result;
        }
    }
}
