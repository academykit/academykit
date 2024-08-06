namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class ZoomLicenseValidator : AbstractValidator<ZoomLicenseRequestModel>
    {
        public ZoomLicenseValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.HostId)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("HostIdRequired"))
                .WithMessage(context => stringLocalizer.GetString("HostIDLength"));
            RuleFor(x => x.LicenseEmail)
                .NotNull()
                .NotEmpty()
                .WithMessage(stringLocalizer.GetString("ZoomLicenseEmailRequired"))
                .WithMessage(context => stringLocalizer.GetString("LicenseEmailLength"));
            RuleFor(x => x.Capacity)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("CapacityRequired"));
        }
    }
}
