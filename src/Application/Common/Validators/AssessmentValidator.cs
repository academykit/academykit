namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class AssessmentValidator : AbstractValidator<AssessmentRequestModel>
    {
        public AssessmentValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("TitleRequired"))
                .MaximumLength(500)
                .WithMessage(context => stringLocalizer.GetString("NameLength500"));

            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .WithMessage(context => stringLocalizer.GetString("DurationGreaterThanZero"));

            RuleFor(x => x.Weightage)
                .GreaterThan(0)
                .WithMessage(context => stringLocalizer.GetString("WeightageGreaterThanZero"));
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("DescriptionLength500");
        }
    }
}
