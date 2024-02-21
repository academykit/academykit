namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Lingtren.Domain.Enums;
    using Microsoft.Extensions.Localization;

    public class AssessmentQuestionValidator : AbstractValidator<AssessmentQuestionRequestModel>
    {
        public AssessmentQuestionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.QuestionName)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("FeedbackNameRequired"))
                .MaximumLength(500)
                .WithMessage(context => stringLocalizer.GetString("NameLength500"));
            RuleFor(x => x.Type)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("FeedbackTypeRequired"))
                .IsInEnum()
                .WithMessage(context => stringLocalizer.GetString("InvalidFeedBackType"));
            RuleFor(x => x.assessmentQuestionOptions)
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("OptionIsRequired"))
                .When(
                    x =>
                        x.Type == AssessmentTypeEnum.SingleChoice
                        || x.Type == AssessmentTypeEnum.MultipleChoice
                );
            RuleFor(x => x.assessmentQuestionOptions)
                .Must(x => x.Count >= 2)
                .WithMessage(context => stringLocalizer.GetString("OptionMoreThanOne"))
                .When(
                    x =>
                        x.Type == AssessmentTypeEnum.SingleChoice
                        || x.Type == AssessmentTypeEnum.MultipleChoice
                );
            RuleForEach(x => x.assessmentQuestionOptions)
                .SetValidator(new AssessmentQuestionOptionValidator(stringLocalizer))
                .When(
                    x =>
                        x.Type == AssessmentTypeEnum.SingleChoice
                        || x.Type == AssessmentTypeEnum.MultipleChoice
                );
        }
    }

    public class AssessmentQuestionOptionValidator
        : AbstractValidator<AssessmentQuestionOptionRequestModel>
    {
        public AssessmentQuestionOptionValidator(
            IStringLocalizer<ValidatorLocalizer> stringLocalizer
        )
        {
            RuleFor(x => x.Option)
                .MaximumLength(5000)
                .WithMessage(context => stringLocalizer.GetString("OptionLengthMustBe5000"));
            RuleFor(x => x.Option)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("OptionIsRequired"));
        }
    }
}
