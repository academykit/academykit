namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;

    public class ZoomLicenseValidator : AbstractValidator<ZoomLicenseRequestModel>
    {
        public ZoomLicenseValidator()
        {
            RuleFor(x => x.HostId).NotNull().NotEmpty().WithMessage("Host id is required");
            RuleFor(x => x.LicenseEmail).NotNull().NotEmpty().WithMessage("Zoom license email is required");
            RuleFor(x => x.Capacity).NotNull().NotEmpty().WithMessage("Capacity is required");
        }
    }
}
