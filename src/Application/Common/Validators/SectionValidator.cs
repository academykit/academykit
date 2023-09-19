namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class SectionValidator : AbstractValidator<SectionRequestModel>
    {
        public SectionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("SectionNameRequired")).MaximumLength(100).WithMessage(context => stringLocalizer.GetString("SectionNameLength"));
        }
    }
}
