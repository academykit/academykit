namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class CourseValidator : AbstractValidator<CourseRequestModel>
    {
        public CourseValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.LevelId).NotNull().NotEmpty().WithMessage("Level is required");
        }
    }
}
