using AcademyKit.Application.ValidatorLocalization;
using AcademyKit.Server.Application.Common.Models.RequestModels;
using AcademyKit.Server.Application.Common.Validators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators;

/// <summary>
/// Validator for InitialSetupRequestModel.
/// </summary>
public class InitialSetupValidator : AbstractValidator<InitialSetupRequestModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitialSetupValidator"/> class.
    /// </summary>
    /// <param name="stringLocalizer">The string localizer for localization support.</param>
    public InitialSetupValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("FirstNameRequired"))
            .MaximumLength(100)
            .WithMessage(context => stringLocalizer.GetString("FirstNameLength100"));

        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("LastNameRequired"))
            .MaximumLength(100)
            .WithMessage(context => stringLocalizer.GetString("LastNameLength100"));

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("EmailRequired"))
            .MaximumLength(100)
            .WithMessage(context => stringLocalizer.GetString("EmailLength100"))
            .Must(email => ValidationHelpers.ValidEmail(email))
            .WithMessage(context => stringLocalizer.GetString("InvalidEmailError"));

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("PasswordRequired"))
            .Length(6, 20)
            .Must(pw => ValidationHelpers.HasValidPassword(pw))
            .WithMessage(context => stringLocalizer.GetString("InvalidPasswordFormat"));

        RuleFor(x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("ConfirmPasswordRequired"));

        RuleFor(x => x.Password)
            .Equal(x => x.ConfirmPassword)
            .WithMessage(context =>
                stringLocalizer.GetString("PasswordAndConfirmPasswordNotMatched")
            );

        RuleFor(x => x.CompanyName)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("CompanyNameRequired"))
            .MaximumLength(250)
            .WithMessage(context => stringLocalizer.GetString("CompanyNameLengthError"));

        RuleFor(x => x.CompanyAddress)
            .MaximumLength(250)
            .WithMessage(context => stringLocalizer.GetString("CompanyAddressLengthError"));

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500)
            .WithMessage(context => stringLocalizer.GetString("LogoUrlLengthError"));
    }
}
