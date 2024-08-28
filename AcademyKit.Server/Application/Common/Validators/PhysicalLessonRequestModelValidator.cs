using System.Text;
using AcademyKit.Application.Common.Models.RequestModels;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators
{
    public class PhysicalLessonReviewRequestModelValidator
        : AbstractValidator<PhysicalLessonReviewRequestModel>
    {
        public PhysicalLessonReviewRequestModelValidator(
            IStringLocalizer<ValidationException> stringLocalizer
        )
        {
            RuleFor(x => x.Message)
                .NotNull()
                .NotEmpty()
                .WithMessage(content => stringLocalizer.GetString("MessageCannotBeEmpty"))
                .MaximumLength(500)
                .WithMessage(context => stringLocalizer.GetString("InvalidReviewLength"))
                .When(x => !x.IsPassed); // Apply this rule when Has Attended is false
            RuleFor(x => x.Message)
                .MaximumLength(500)
                .WithMessage(context => stringLocalizer.GetString("InvalidReviewLength"))
                .When(x => !string.IsNullOrEmpty(x.Message)); // Apply this rule when Message is not null or empty
        }
    }
}
