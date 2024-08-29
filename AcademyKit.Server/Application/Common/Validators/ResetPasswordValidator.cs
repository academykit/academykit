namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.ValidatorLocalization;
    using AcademyKit.Server.Application.Common.Validators;
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
                .Must(pw => ValidationHelpers.HasValidPassword(pw))
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
    }
}
