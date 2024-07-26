namespace AcademyKit.Application.Common.Validators
{
    using System.Text.RegularExpressions;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

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
                .Must(email => ValidEmail(email))
                .WithMessage(context => stringLocalizer.GetString("InvalidEmailError"));
            RuleFor(x => x.MobileNumber)
                .Must(mobileNumber =>
                    string.IsNullOrWhiteSpace(mobileNumber) || ValidMobileNumber(mobileNumber)
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

        public static bool ValidEmail(string email)
        {
            const string emailRegex =
                @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
                + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
                + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            return new Regex(emailRegex).IsMatch(email);
        }

        public static bool ValidMobileNumber(string mobileNumber)
        {
            const string mobilePattern = @"^[+\d]+$";
            return new Regex(mobilePattern).IsMatch(mobileNumber);
        }
    }
}
