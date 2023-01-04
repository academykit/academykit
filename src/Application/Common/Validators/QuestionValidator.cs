namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;

    public class QuestionValidator : AbstractValidator<QuestionRequestModel>
    {
        public QuestionValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Question name is required.").MaximumLength(500)
                        .WithMessage("Name length must be less than or equal to 500 characters");
            RuleFor(x => x.Hints).MaximumLength(5000).WithMessage("Question Hints length must be less than or equal to 5000 characters");
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("Question Description length must be less than or equal to 5000 characters");
            RuleFor(x => x.Type).NotNull().WithMessage("Question type is required.");
            RuleFor(x => x.Answers).NotNull().WithMessage("Option is required.");
            RuleFor(x => x.Answers).Must(x => x.Count >= 2).WithMessage("Option should be more than one.");
            RuleFor(x => x.Answers).Must(a => a.Any(b => b.IsCorrect)).WithMessage("At least one option should be provided as correct.");
            RuleFor(x => x.Answers.Count(a => a.IsCorrect)).LessThanOrEqualTo(1).When(x => x.Type == QuestionTypeEnum.SingleChoice)
                                            .WithMessage("Question type with single choice can have only one option correct.");
            RuleForEach(x => x.Answers).SetValidator(new QuestionOptionValidator());
        }
    }

    public class QuestionOptionValidator : AbstractValidator<QuestionOptionRequestModel>
    {
        public QuestionOptionValidator()
        {
            RuleFor(x => x.Option).MaximumLength(5000).WithMessage("Option length must be less than or equal to 5000 characters");
            RuleFor(x => x.Option).NotEmpty().NotNull().WithMessage("Question option is required.");
        }
    }
}
