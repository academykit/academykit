namespace Lingtren.Application.Common.Validators
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    public class ZoomSettingValidator : AbstractValidator<ZoomSettingRequestModel>
    {
        public ZoomSettingValidator()
        {
            RuleFor(x => x.ApiKey).NotNull().NotEmpty().WithMessage("Api key is required");
            RuleFor(x => x.SecretKey).NotNull().NotEmpty().WithMessage("Secret key is required");
            RuleFor(x => x.IsRecordingEnabled).NotNull().NotEmpty().WithMessage("Is Recording Enabled is required");
        }
    }
}
