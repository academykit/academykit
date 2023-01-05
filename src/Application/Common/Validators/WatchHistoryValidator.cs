namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;

    public class WatchHistoryValidator : AbstractValidator<WatchHistoryRequestModel>
    {
        public WatchHistoryValidator()
        {
            this.RuleFor(x => x.CourseIdentity).NotNull().NotEmpty().WithMessage("Course identity is required.");
            this.RuleFor(x => x.LessonIdentity).NotNull().NotEmpty().WithMessage("Lesson identity is required.");
            this.RuleFor(x => x.WatchedPercentage).ExclusiveBetween(0, 101).WithMessage("Percentage should be in between 0 to 100.");
        }
    }
}