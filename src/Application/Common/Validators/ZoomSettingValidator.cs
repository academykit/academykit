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
            RuleFor(x => x.ApiKey).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("APIKeyRequired")).MaximumLength(100).WithMessage(context => stringLocalizer.GetString("ApiKeyLength100"));
            RuleFor(x => x.ApiSecret).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("ApiSecretRequired")).MaximumLength(100).WithMessage(context => stringLocalizer.GetString("ApiSecretKeyLength100"));
            RuleFor(x => x.SdkKey).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("SDKKeyRequired")).MaximumLength(100).WithMessage(context => stringLocalizer.GetString("SdkKeyLength100"));
            RuleFor(x => x.SdkSecret).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("SdkSecretRequired")).MaximumLength(100).WithMessage(context => stringLocalizer.GetString("SdkLength100"));
            RuleFor(x => x.IsRecordingEnabled).NotNull().WithMessage(context => stringLocalizer.GetString("IsRecordingTrue"));
        }
    }
}
