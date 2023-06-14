namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using System.Text.RegularExpressions;

    public class UserValidator : AbstractValidator<UserRequestModel>
    {
        public UserValidator()
        {
            RuleSet("Add", () =>
            {
                RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First name is required.").MaximumLength(100).WithMessage("First name length must be less than or equal to 100 characters.");
                RuleFor(x => x.MiddleName).MaximumLength(100).WithMessage("Middle name length must be less than or equal to 100 characters.");
                RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last name is required.").MaximumLength(100).WithMessage("Last name length must be less than or equal to 100 characters.");
                RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required").MaximumLength(100).WithMessage("Email length must be less than or equal to 100 characters.")
                .Must(email => ValidEmail(email)).WithMessage("Invalid email format.");
                RuleFor(x => x.MobileNumber).MaximumLength(50).WithMessage("Mobile number length must be less than or equal to 50 characters.");
                RuleFor(x => x.Address).MaximumLength(200).WithMessage("Address length must be less than or equal to 200 characters.");
                RuleFor(x => x.Profession).MaximumLength(200).WithMessage("Profession length must be less than or equal to 200 characters.");
                RuleFor(x => x.Bio).Must(bio => RemoveHtmlTags(bio).Length <= 200).WithMessage("Bio length must be less than or equal to 200 characters.");
                RuleFor(x => x.PublicUrls).MaximumLength(200).WithMessage("Public url length must be less than or equal to 2000 characters.");
                RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage("Role is required.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First name is required.").MaximumLength(100).WithMessage("First name length must be less than or equal to 100 characters.");
                RuleFor(x => x.MiddleName).MaximumLength(100).WithMessage("Middle name length must be less than or equal to 100 characters.");
                RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last name is required.").MaximumLength(100).WithMessage("First name length must be less than or equal to 100 characters.");
                RuleFor(x => x.MobileNumber).MaximumLength(50).WithMessage("Mobile number length must be less than or equal to 50 characters.");
                RuleFor(x => x.Address).MaximumLength(200).WithMessage("Address length must be less than or equal to 200 characters.");
                RuleFor(x => x.Profession).MaximumLength(200).WithMessage("Profession length must be less than or equal to 200 characters.");
                RuleFor(x => x.PublicUrls).MaximumLength(200).WithMessage("Public url length must be less than or equal to 2000 characters.");
                RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage("Role is required.");
                RuleFor(x => x.Bio).Must(bio => RemoveHtmlTags(bio).Length <= 200).WithMessage("Bio length must be less than or equal to 200 characters.");
            });
        }

        /// <summary>
        /// Handel to remove html tags from bio
        /// </summary>
        /// <param name="bio">User's bio</param>
        /// <returns>string</returns>
        private string RemoveHtmlTags(string bio)
        {
             string textonly = Regex.Replace(bio, "<.*?>", "");
             return textonly;
            
        }
        public static bool ValidEmail(string email)
        {
            const string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                   @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                      @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            return new Regex(emailRegex).IsMatch(email);
        }
    }
}
