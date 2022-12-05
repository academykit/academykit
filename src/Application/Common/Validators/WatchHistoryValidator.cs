namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;

    public class WatchHistoryValidator : AbstractValidator<WatchHistoryRequestModel>
    {
        public WatchHistoryValidator()
        {
            this.RuleFor(x => x.CourseIdentity).NotNull().NotEmpty().WithMessage("Required course identity");
            this.RuleFor(x => x.LessonIdentity).NotNull().NotEmpty().WithMessage("Required lesson identity");
            this.RuleFor(x => x.WatchedPercentage).ExclusiveBetween(0, 101).WithMessage("Percentage between 0 to 100");
        }
    }
}