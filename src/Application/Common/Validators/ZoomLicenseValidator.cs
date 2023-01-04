namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;

    public class ZoomLicenseValidator : AbstractValidator<ZoomLicenseRequestModel>
    {
        public ZoomLicenseValidator()
        {
            RuleFor(x => x.HostId).NotNull().NotEmpty().WithMessage("Host id is required").WithMessage("Host id length must be less than or equal to 50 characters");
            RuleFor(x => x.LicenseEmail).NotNull().NotEmpty().WithMessage("Zoom license email is required").WithMessage("License Email length must be less than or equal to 50 characters");
            RuleFor(x => x.Capacity).NotNull().NotEmpty().WithMessage("Capacity is required");
        }
    }
}
