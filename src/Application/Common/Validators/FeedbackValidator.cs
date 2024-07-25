namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Lingtren.Domain.Enums;
    using Microsoft.Extensions.Localization;

    public class FeedbackValidator : AbstractValidator<FeedbackRequestModel>
    {
        public FeedbackValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.LessonId)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("LessonIdRequired"));
            RuleFor(x => x.Name)
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
            RuleFor(x => x.Answers)
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("OptionIsRequired"))
                .When(x =>
                    x.Type == FeedbackTypeEnum.SingleChoice
                    || x.Type == FeedbackTypeEnum.MultipleChoice
                );
            RuleFor(x => x.Answers)
                .Must(x => x.Count >= 2)
                .WithMessage(context => stringLocalizer.GetString("OptionMoreThanOne"))
                .When(x =>
                    x.Type == FeedbackTypeEnum.SingleChoice
                    || x.Type == FeedbackTypeEnum.MultipleChoice
                );
            RuleForEach(x => x.Answers)
                .SetValidator(new FeedbackQuestionOptionValidator(stringLocalizer))
                .When(x =>
                    x.Type == FeedbackTypeEnum.SingleChoice
                    || x.Type == FeedbackTypeEnum.MultipleChoice
                );
        }
    }

    public class FeedbackQuestionOptionValidator
        : AbstractValidator<FeedbackQuestionOptionRequestModel>
    {
        public FeedbackQuestionOptionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
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
