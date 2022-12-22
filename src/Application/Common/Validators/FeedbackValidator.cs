namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;

    public class FeedbackValidator : AbstractValidator<FeedbackRequestModel>
    {
        public FeedbackValidator()
        {
            RuleFor(x => x.LessonId).NotEmpty().NotNull().WithMessage("Lesson id is required");
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Feedback name is required.").MaximumLength(500)
                        .WithMessage("Name length must be less than or equal to 500 characters");
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("Description length must be less than or equal to 5000 characters");
            RuleFor(x => x.Type).NotNull().WithMessage("Feedback type is required.");
            RuleFor(x => x.Answers).NotNull().WithMessage("Option is required.")
                                    .When(x => x.Type == FeedbackTypeEnum.SingleChoice || x.Type == FeedbackTypeEnum.MultipleChoice);
            RuleFor(x => x.Answers).Must(x => x.Count >= 2).WithMessage("Option should be more than one.")
                                    .When(x => x.Type == FeedbackTypeEnum.SingleChoice || x.Type == FeedbackTypeEnum.MultipleChoice);
            RuleForEach(x => x.Answers).SetValidator(new FeedbackQuestionOptionValidator())
                                    .When(x => x.Type == FeedbackTypeEnum.SingleChoice || x.Type == FeedbackTypeEnum.MultipleChoice);
        }
    }

    public class FeedbackQuestionOptionValidator : AbstractValidator<FeedbackQuestionOptionRequestModel>
    {
        public FeedbackQuestionOptionValidator()
        {
            RuleFor(x => x.Option).MaximumLength(5000).WithMessage("Option length must be less than or equal to 5000 characters");
            RuleFor(x => x.Option).NotEmpty().NotNull().WithMessage("Feed Question option is required.");
        }
    }
}
