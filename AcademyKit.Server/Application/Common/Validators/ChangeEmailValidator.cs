using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators;

public class ChangeEmailValidator : AbstractValidator<ChangeEmailRequestModel>
{
    public ChangeEmailValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
    {
        RuleFor(x => x.NewEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("NewPasswordRequired"))
            .Length(6, 100)
            .Must(email => ValidationHelpers.ValidEmail(email))
            .WithMessage(context => stringLocalizer.GetString("InvalidEmailError"));
        RuleFor(x => x.OldEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("OldEmailRequired"));
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("PasswordRequired"));
        RuleFor(x => x.ConfirmEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("ConfirmMailRequired"));
        RuleFor(x => x.NewEmail)
            .Equal(x => x.ConfirmEmail)
            .WithMessage("NewEmailConformedRequired");
    }
}
