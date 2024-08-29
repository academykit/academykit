namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using AcademyKit.Server.Application.Common.Validators;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class UserUpdateValidator : AbstractValidator<UserUpdateRequestModel>
    {
        public UserUpdateValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
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
            RuleFor(x => x.MobileNumber)
                .Must(mobileNumber =>
                    string.IsNullOrWhiteSpace(mobileNumber)
                    || ValidationHelpers.ValidMobileNumber(mobileNumber)
                )
                .When(x => !string.IsNullOrWhiteSpace(x.MobileNumber))
                .WithMessage(context => stringLocalizer.GetString("InvalidMobileNumber"))
                .MaximumLength(50)
                .WithMessage(context => stringLocalizer.GetString("MobileNumberLength50"));
            RuleFor(x => x.Address)
                .MaximumLength(200)
                .WithMessage(context => stringLocalizer.GetString("AddressLength200"));
            RuleFor(x => x.Profession)
                .MaximumLength(200)
                .WithMessage(context => stringLocalizer.GetString("ProfessionLength200"));
            RuleFor(x => x.PublicUrls)
                .MaximumLength(200)
                .WithMessage(context => stringLocalizer.GetString("PublicUrlLength2000"));
            RuleFor(x => x.Role)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("RoleRequired"));
            RuleFor(x => x.Bio)
                .Must(bio => Helpers.CommonHelper.RemoveHtmlTags(bio).Length <= 200)
                .WithMessage(context => stringLocalizer.GetString("BioLength200"));
            RuleFor(x => x.FatherName)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("FatherNameRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("FatherNameLength100"));
            RuleFor(x => x.MotherName)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("MotherNameRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("MotherNameLength100"));
            RuleFor(x => x.SpouseName)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("SpouseNameRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("SpouseNameLength100"));
            RuleFor(x => x.GrandFatherName)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("GrandFatherNameRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("GrandFatherNameLength100"));
            RuleFor(x => x.MemberPhone)
                .Must(memberPhone =>
                    string.IsNullOrWhiteSpace(memberPhone)
                    || ValidationHelpers.ValidMobileNumber(memberPhone)
                )
                .When(x => !string.IsNullOrWhiteSpace(x.MemberPhone))
                .WithMessage(context => stringLocalizer.GetString("InvalidMobileNumber"))
                .MaximumLength(50)
                .WithMessage(context => stringLocalizer.GetString("MobileNumberLength50"));
            RuleFor(x => x.MemberPermanentAddress)
                .MaximumLength(200)
                .WithMessage(context => stringLocalizer.GetString("AddressLength200"));
            RuleFor(x => x.MemberCurrentAddress)
                .MaximumLength(200)
                .WithMessage(context => stringLocalizer.GetString("AddressLength200"));
        }
    }
}
