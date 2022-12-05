namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    using System.Text.RegularExpressions;

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.NewPassword).NotNull().NotEmpty().WithMessage("New password is required").Length(6, 20)
                     .Must(pw => HasValidPassword(pw)).WithMessage("Password should contains at least one lowercase, one uppercase, one digit and one symbol");
            RuleFor(x => x.ConfirmPassword).NotNull().NotEmpty().WithMessage("Confirm password is required");
            RuleFor(x => x.NewPassword).Equal(x => x.ConfirmPassword).WithMessage("New password and Confirm password does not matched");
        }

        public static bool HasValidPassword(string pw)
        {
            var lowercase = new Regex("[a-z]+");
            var uppercase = new Regex("[A-Z]+");
            var digit = new Regex("(\\d)+");
            var symbol = new Regex("(\\W)+");
            return lowercase.IsMatch(pw) && uppercase.IsMatch(pw) && digit.IsMatch(pw) && symbol.IsMatch(pw);
        }
    }
}
