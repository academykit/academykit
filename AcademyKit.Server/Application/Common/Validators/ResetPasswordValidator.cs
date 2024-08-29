namespace AcademyKit.Application.Common.Validators
{
    using System.Text.RegularExpressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequestModel>
    {
        public ResetPasswordValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.NewPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("NewPasswordRequired"))
                .Length(6, 20)
                .Must(pw => HasValidPassword(pw))
                .WithMessage(context => stringLocalizer.GetString("InvalidPasswordFormat"));
            RuleFor(x => x.ConfirmPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ConfirmPasswordRequired"));
            RuleFor(x => x.PasswordChangeToken)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("PasswordChangeTokenIsRequired"));
            RuleFor(x => x.NewPassword)
                .Equal(x => x.ConfirmPassword)
                .WithMessage(context => stringLocalizer.GetString("OldAndNewPasswordDoesNotMatch"));
        }

        public static bool HasValidPassword(string pw)
        {
            var lowercase = new Regex("[a-z]+");
            var uppercase = new Regex("[A-Z]+");
            var digit = new Regex("(\\d)+");
            var symbol = new Regex("(\\W)+");
            return lowercase.IsMatch(pw)
                && uppercase.IsMatch(pw)
                && digit.IsMatch(pw)
                && symbol.IsMatch(pw);
        }
    }
}
