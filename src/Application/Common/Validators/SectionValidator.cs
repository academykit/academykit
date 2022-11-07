using System.ComponentModel;
namespace Lingtren.Application.Common.Validators
{
    using Lingtren.Application.Common.Models.RequestModels;
    using FluentValidation;
    public class SectionValidator : AbstractValidator<SectionRequestModel>
    {
        public SectionValidator()
        {
            RuleFor(x => x.CourseId).NotNull().NotEmpty().WithMessage("Course id required");
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Required name");
        }
    }
}