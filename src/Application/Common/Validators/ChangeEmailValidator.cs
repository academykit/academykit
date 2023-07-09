namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;
    using System.Text.RegularExpressions;

    public class ChangeEmailValidator : AbstractValidator<ChangeEmailRequestModel>
    {
        public ChangeEmailValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.NewEmail).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("NewPasswordRequired")).Length(6, 100)
                     .Must(email => ValidEmail(email)).WithMessage(context => stringLocalizer.GetString("InvalidEmailError"));
            RuleFor(x => x.OldEmail).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("OldEmailRequired"));
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("PasswordRequired"));
            RuleFor(x => x.ConfirmEmail).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("ConfirmMailRequired"));
            RuleFor(x => x.NewEmail).Equal(x => x.ConfirmEmail).WithMessage("NewEmailConformedRequired");
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
