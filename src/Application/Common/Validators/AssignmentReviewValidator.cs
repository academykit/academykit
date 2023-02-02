namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class AssignmentReviewValidator : AbstractValidator<AssignmentReviewRequestModel>
    {
        public AssignmentReviewValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("UserId is required.");
            RuleFor(x => x.Marks).InclusiveBetween(1, 100).WithMessage("Marks must be in between 1 to 100.");
            RuleFor(x => x.Review).MaximumLength(500).WithMessage("Review length should be less than or equal to 500 characters.");
        }
    }
}
