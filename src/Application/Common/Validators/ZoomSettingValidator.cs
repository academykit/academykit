namespace Lingtren.Application.Common.Validators
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    public class ZoomSettingValidator : AbstractValidator<ZoomSettingRequestModel>
    {
        public ZoomSettingValidator()
        {
            RuleFor(x => x.ApiKey).NotNull().NotEmpty().WithMessage("Api key is required").MaximumLength(100).WithMessage("Api key length must be less than or equal to 100 characters");
            RuleFor(x => x.ApiSecret).NotNull().NotEmpty().WithMessage("Api secret is required").MaximumLength(100).WithMessage("Api Secret length must be less than or equal to 100 characters");
            RuleFor(x => x.SdkKey).NotNull().NotEmpty().WithMessage("Sdk Key is required").MaximumLength(100).WithMessage("Sdk Key length must be less than or equal to 100 characters");
            RuleFor(x => x.SdkSecret).NotNull().NotEmpty().WithMessage("Sdk Secret is required").MaximumLength(100).WithMessage("Sdk Secret name length must be less than or equal to 100 characters");
            RuleFor(x => x.IsRecordingEnabled).NotNull().WithMessage("Is Recording Enabled is required");
        }
    }
}
