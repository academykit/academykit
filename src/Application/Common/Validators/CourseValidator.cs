namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class CourseValidator : AbstractValidator<CourseRequestModel>
    {
        public CourseValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required").MaximumLength(250).WithMessage("Name length must be less than or equal to 250 characters");
            RuleFor(x => x.LevelId).NotNull().NotEmpty().WithMessage("Level is required");
            RuleFor(x => x.GroupId).NotNull().NotEmpty().WithMessage("Please select group");
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("Description length must be less than or equal to 5000 characters");
        }
    }
}
