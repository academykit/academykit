namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class WatchHistoryValidator : AbstractValidator<WatchHistoryRequestModel>
    {
        public WatchHistoryValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.CourseIdentity)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("TrainingIdRequired"));
            RuleFor(x => x.LessonIdentity)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("LessonIdRequired"));
            RuleFor(x => x.WatchedPercentage)
                .ExclusiveBetween(0, 101)
                .WithMessage(context => stringLocalizer.GetString("ValidPercentageRange"));
        }
    }
}
