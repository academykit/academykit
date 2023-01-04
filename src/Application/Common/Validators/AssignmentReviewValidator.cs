namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class AssignmentReviewValidator : AbstractValidator<AssignmentReviewRequestModel>
    {
        public AssignmentReviewValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("User id is required.");
            RuleFor(x => x.Marks).NotEmpty().NotNull().WithMessage("Marks is required.").InclusiveBetween(1, 100).WithMessage("Value must be in between 1 to 100");
            RuleFor(x => x.Review).MaximumLength(500).WithMessage("Review length should be less than or equal to 500.");
        }
    }
}
