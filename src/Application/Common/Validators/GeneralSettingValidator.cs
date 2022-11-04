namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class GeneralSettingValidator : AbstractValidator<GeneralSettingRequestModel>
    {
        public GeneralSettingValidator()
        {
            RuleFor(x => x.CompanyName).NotNull().NotEmpty().WithMessage("Company name is required");
            RuleFor(x => x.CompanyAddress).NotNull().NotEmpty().WithMessage("Company address is required");
            RuleFor(x => x.CompanyContactNumber).NotNull().NotEmpty().WithMessage("Company contact number is required");
            RuleFor(x => x.LogoUrl).NotNull().NotEmpty().WithMessage("Logo is required");
        }
    }
}
