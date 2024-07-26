namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class AssignmentReviewValidator : AbstractValidator<AssignmentReviewRequestModel>
    {
        public AssignmentReviewValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("UserIDrequired"));
            RuleFor(x => x.Review)
                .MaximumLength(500)
                .WithMessage(context => stringLocalizer.GetString("InvalidReviewLength"));
        }
    }
}
