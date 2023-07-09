namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class WatchHistoryValidator : AbstractValidator<WatchHistoryRequestModel>
    {
        public WatchHistoryValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            this.RuleFor(x => x.CourseIdentity).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("TrainingIdRequired"));
            this.RuleFor(x => x.LessonIdentity).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("LessonIdRequired"));
            this.RuleFor(x => x.WatchedPercentage).ExclusiveBetween(0, 101).WithMessage(context => stringLocalizer.GetString("ValidPercentageRange"));
        }
    }
}