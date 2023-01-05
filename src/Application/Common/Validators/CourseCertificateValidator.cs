namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class CourseCertificateValidator : AbstractValidator<CourseCertificateRequestModel>
    {
        public CourseCertificateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().NotNull().WithMessage("User id is required.").MaximumLength(100).WithMessage("Title length must be less than or equal to 100.");
            RuleFor(x => x.EventStartDate).NotEmpty().NotNull().WithMessage("Event start date is required.");
            RuleFor(x => x.EventEndDate).NotEmpty().NotNull().WithMessage("Event end date is required.");
        }
    }
}
