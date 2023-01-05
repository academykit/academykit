namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class GeneralSettingValidator : AbstractValidator<GeneralSettingRequestModel>
    {
        public GeneralSettingValidator()
        {
            RuleFor(x => x.CompanyName).NotNull().NotEmpty().WithMessage("Company name is required.").MaximumLength(250).WithMessage("Company name length must be less than or equal to 250 characters.");
            RuleFor(x => x.CompanyAddress).NotNull().NotEmpty().WithMessage("Company address is required.").MaximumLength(250).WithMessage("Company address length must be less than or equal to 250 characters.");
            RuleFor(x => x.CompanyContactNumber).NotNull().NotEmpty().WithMessage("Company contact number is required.").MaximumLength(30).WithMessage("Company contact must be less than or equal to 30 characters.");
            RuleFor(x => x.LogoUrl).NotNull().NotEmpty().WithMessage("Logo is required").MaximumLength(500).WithMessage("Logo url length must be less than or equal to 500 characters.");
            RuleFor(x => x.EmailSignature).MaximumLength(1000).WithMessage("Email signature length must be less than or equal to 1000 characters.");
        }
    }
}
