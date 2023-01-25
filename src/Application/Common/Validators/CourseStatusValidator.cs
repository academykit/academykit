using System.Data;
namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;

    public class CourseStatusValidator : AbstractValidator<CourseStatusRequestModel>
    {
        public CourseStatusValidator()
        {
            RuleFor(x => x.Identity).NotNull().NotEmpty().WithMessage("Required course identity.");
            RuleFor(x => x.Status).NotNull().NotEmpty().WithMessage("Required course status.");
            RuleFor(x => x).Must(x => x.Status == CourseStatus.Rejected && string.IsNullOrEmpty(x.Message)).WithMessage("Required message");
        }
    }
}