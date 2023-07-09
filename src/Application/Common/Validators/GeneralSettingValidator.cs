namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class GeneralSettingValidator : AbstractValidator<GeneralSettingRequestModel>
    {
        public GeneralSettingValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.CompanyName).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("CompanyNameRequired")).MaximumLength(250).WithMessage(context => stringLocalizer.GetString("CompanyNameLengthError"));
            RuleFor(x => x.CompanyAddress).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("CompanyAddressRequired")).MaximumLength(250).WithMessage(context => stringLocalizer.GetString("CompanyAddressLengthError"));
            RuleFor(x => x.CompanyContactNumber).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("CompanyContactNumberRequired")).MaximumLength(30).WithMessage(context => stringLocalizer.GetString("CompanyContactNumberLengthError"));
            RuleFor(x => x.LogoUrl).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("LogoRequired")).MaximumLength(500).WithMessage(context => stringLocalizer.GetString("LogoUrlRequired"));
            RuleFor(x => x.EmailSignature).MaximumLength(1000).WithMessage(context => stringLocalizer.GetString("EmailSignatureLengthError"));
        }
    }
}
