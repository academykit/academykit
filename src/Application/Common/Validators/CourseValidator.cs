namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class CourseValidator : AbstractValidator<CourseRequestModel>
    {
        public CourseValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Name)
                .Must(name => Helpers.CommonHelper.RemoveHtmlTags(name).Length <= 100)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("NameRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("NameLengthError"));
            RuleFor(x => x.LevelId)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("LevelRequired"));
            RuleFor(x => x.GroupId)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("SelectGroup"));
            RuleFor(x => x.Description)
                .MaximumLength(5000)
                .WithMessage(context => stringLocalizer.GetString("DescriptionLength500"));
            RuleFor(x => x).SetValidator(new CourseDateValidator(stringLocalizer));
        }
    }

    public class CourseDateValidator : AbstractValidator<CourseRequestModel>
    {
        public CourseDateValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x)
                .Must(x =>
                    x.IsUnlimitedEndDate == true
                        ? x.StartDate != default
                        : x.StartDate != default && x.EndDate != default && x.EndDate > x.StartDate
                )
                .WithMessage(context => stringLocalizer.GetString("EnddateMustBeGreater"));
        }
    }
}
