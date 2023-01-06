namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class CourseTeacherValidator : AbstractValidator<CourseTeacherRequestModel>
    {
        public CourseTeacherValidator()
        {
            RuleFor(x => x.CourseIdentity).NotNull().NotEmpty().WithMessage("Training identity is required.");
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required.");
        }
    }
}
