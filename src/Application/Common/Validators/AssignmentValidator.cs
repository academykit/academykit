namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;

    public class AssignmentValidator : AbstractValidator<AssignmentRequestModel>
    {
        public AssignmentValidator()
        {
            RuleFor(x => x.LessonId).NotEmpty().NotNull().WithMessage("LessonId is required");
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Assignment name is required.").MaximumLength(500)
                        .WithMessage("Name length must be less than or equal to 500 characters.");
            RuleFor(x => x.Hints).MaximumLength(5000).WithMessage("Hints length must be less than or equal to 5000 characters.");
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("Description length must be less than or equal to 5000 characters.");
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("Assignment type is required.").IsInEnum().WithMessage("Invalid assignment type.");
            RuleFor(x => x.Answers).NotNull().WithMessage("Option is required.")
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
            RuleFor(x => x.Answers).Must(x => x.Count >= 2).WithMessage("Option should be more than one.")
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
            RuleFor(x => x.Answers).Must(a => a.Any(b => b.IsCorrect)).WithMessage("At least one option should be provided as correct.")
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
            RuleForEach(x => x.Answers).SetValidator(new AssignmentQuestionOptionValidator())
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
        }
    }

    public class AssignmentQuestionOptionValidator : AbstractValidator<AssignmentQuestionOptionRequestModel>
    {
        public AssignmentQuestionOptionValidator()
        {
            RuleFor(x => x.Option).MaximumLength(5000).WithMessage("Option length must be less than or equal to 5000 characters.");
            RuleFor(x => x.Option).NotEmpty().NotNull().WithMessage("Assignment Question option is required.");
        }
    }
}
