namespace Lingtren.Application.Common.Validators
{
    using System.Text.RegularExpressions;
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestModel>
    {
        public ChangePasswordValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.NewPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("NewPasswordRequired"))
                .Length(6, 20)
                .Must(pw => HasValidPassword(pw))
                .WithMessage("InvalidPasswordFormat");
            RuleFor(x => x.ConfirmPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ConformPassword"));
            RuleFor(x => x.NewPassword)
                .Equal(x => x.ConfirmPassword)
                .WithMessage(context => stringLocalizer.GetString("OldAndNewPasswordDoesnotMatch"));
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
