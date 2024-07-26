namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class SectionValidator : AbstractValidator<SectionRequestModel>
    {
        public SectionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("SectionNameRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("SectionNameLength"));
        }
    }
}
