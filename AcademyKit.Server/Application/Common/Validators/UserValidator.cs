using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators;

public class UserValidator : AbstractValidator<UserRequestModel>
{
    public UserValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("FirstNameRequired"))
            .MaximumLength(100)
            .WithMessage(context => stringLocalizer.GetString("FirstNameLength100"));
        RuleFor(x => x.MiddleName)
            .MaximumLength(100)
            .WithMessage(context => stringLocalizer.GetString("MiddleNameLength100"));
        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("LastNAmeRequired"))
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
        RuleFor(x => x.MobileNumber)
            .Must(mobileNumber =>
                string.IsNullOrWhiteSpace(mobileNumber)
                || ValidationHelpers.ValidMobileNumber(mobileNumber)
            )
            .When(x => !string.IsNullOrWhiteSpace(x.MobileNumber))
            .WithMessage(context => stringLocalizer.GetString("InvalidMobileNumber"))
            .MaximumLength(15)
            .WithMessage(context => stringLocalizer.GetString("MobileNumberLength15"));
        RuleFor(x => x.Address)
            .MaximumLength(200)
            .WithMessage(context => stringLocalizer.GetString("AddressLength200"));
        RuleFor(x => x.Profession)
            .MaximumLength(200)
            .WithMessage(context => stringLocalizer.GetString("ProfessionLength200"));
        RuleFor(x => x.Bio)
            .Must(bio => Helpers.CommonHelper.RemoveHtmlTags(bio).Length <= 200)
            .WithMessage(context => stringLocalizer.GetString("BioLength200"));
        RuleFor(x => x.PublicUrls)
            .MaximumLength(200)
            .WithMessage(context => stringLocalizer.GetString("PublicUrlLength2000"));
        RuleFor(x => x.Role)
            .NotNull()
            .NotEmpty()
            .WithMessage(context => stringLocalizer.GetString("RoleRequired"));
    }
}
