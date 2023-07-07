namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class AssignmentReviewValidator : AbstractValidator<AssignmentReviewRequestModel>
    {
        public AssignmentReviewValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage(stringLocalizer.GetString("UserIDrequired"));
            RuleFor(x => x.Marks).InclusiveBetween(1, 100).WithMessage(stringLocalizer.GetString("InvalidAssignmentMarks"));
            RuleFor(x => x.Review).MaximumLength(500).WithMessage(stringLocalizer.GetString("InvalidReviewLength"));
        }
    }
}
