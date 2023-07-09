namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class LoginValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("EmailRequired"));
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("PasswordRequired"));
        }
    }
}
