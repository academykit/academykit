namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using AcademyKit.Domain.Enums;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class AssessmentStatusValidator : AbstractValidator<AssessmentStatusRequestModel>
    {
        public AssessmentStatusValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Identity)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("AssessmentIdRequired"));
            RuleFor(x => x.Status)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("AssessmentStatusRequired"));
            RuleFor(x => x.Message)
                .NotNull()
                .NotEmpty()
                .When(x => x.Status == AssessmentStatus.Rejected)
                .WithMessage(context => stringLocalizer.GetString("RejectMessageRequired"));
        }
    }
}
