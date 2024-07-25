namespace Lingtren.Application.Common.Validators
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class ZoomSettingValidator : AbstractValidator<ZoomSettingRequestModel>
    {
        public ZoomSettingValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.OAuthAccountId)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("OAuthAccountIdRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("OAuthAccountIdLength100"));
            RuleFor(x => x.OAuthClientId)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("OAuthClientIdRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("OAuthClientIdLength100"));
            RuleFor(x => x.OAuthClientSecret)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("OAuthClientSecretRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("OAuthClientSecretLength100"));
            RuleFor(x => x.SdkKey)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("SDKKeyRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("SdkKeyLength100"));
            RuleFor(x => x.SdkSecret)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("SdkSecretRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("SdkLength100"));
            RuleFor(x => x.IsRecordingEnabled)
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("IsRecordingTrue"));
        }
    }
}
