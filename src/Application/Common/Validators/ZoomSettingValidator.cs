namespace Lingtren.Application.Common.Validators
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    public class ZoomSettingValidator : AbstractValidator<ZoomSettingRequestModel>
    {
        public ZoomSettingValidator()
        {
            RuleFor(x => x.ApiKey).NotNull().NotEmpty().WithMessage("Api key is required");
            RuleFor(x => x.ApiSecret).NotNull().NotEmpty().WithMessage("Api secret is required");
            RuleFor(x => x.SdkKey).NotNull().NotEmpty().WithMessage("Sdk Key is required");
            RuleFor(x => x.SdkSecret).NotNull().NotEmpty().WithMessage("Sdk secret is required");
            RuleFor(x => x.IsRecordingEnabled).NotNull().WithMessage("Is Recording Enabled is required");
        }
    }
}
