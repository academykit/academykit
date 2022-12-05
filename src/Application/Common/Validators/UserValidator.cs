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
                RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First name is required");
                RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last name is required");
                RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required")
                .Must(email => ValidEmail(email)).WithMessage("Invalid email format");
                RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage("Role is required");
            });

            RuleSet("Update", () =>
            {
                RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First name is required");
                RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last name is required");
            });
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
