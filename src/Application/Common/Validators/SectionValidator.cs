namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class SectionValidator : AbstractValidator<SectionRequestModel>
    {
        public SectionValidator()
        {
            RuleFor(x => x.CourseIdentity).NotNull().NotEmpty().WithMessage("Course id or slug is required");
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Section name is required");
        }
    }
}