namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using System.Text.RegularExpressions;

    public class ChangeEmailValidator : AbstractValidator<ChangeEmailRequestModel>
    {
        public ChangeEmailValidator()
        {
            RuleFor(x => x.NewEmail).NotNull().NotEmpty().WithMessage("New password is required").Length(6, 100)
                     .Must(email => ValidEmail(email)).WithMessage("Invalid email format");
            RuleFor(x => x.OldEmail).NotNull().NotEmpty().WithMessage("old email is required");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("password email is required");
            RuleFor(x => x.ConfirmEmail).NotNull().NotEmpty().WithMessage("Confirm email is required");
            RuleFor(x => x.NewEmail).Equal(x => x.ConfirmEmail).WithMessage("New email and Confirm email does not matched");
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
