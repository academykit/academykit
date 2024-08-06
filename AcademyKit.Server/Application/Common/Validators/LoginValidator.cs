namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class LoginValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("EmailRequired"));
            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("PasswordRequired"));
        }
    }
}
