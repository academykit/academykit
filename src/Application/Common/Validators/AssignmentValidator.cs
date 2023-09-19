namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Lingtren.Domain.Enums;
    using Microsoft.Extensions.Localization;

    public class AssignmentValidator : AbstractValidator<AssignmentRequestModel>
    {
        public AssignmentValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.LessonId).NotEmpty().NotNull().WithMessage(context => stringLocalizer.GetString("LessonIdRequired"));
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage(context => stringLocalizer.GetString("AssignmentRequired")).MaximumLength(500)
                        .WithMessage(context => stringLocalizer.GetString("NameLength500"));
            RuleFor(x => x.Hints).MaximumLength(5000).WithMessage(context => stringLocalizer.GetString("HintLength500"));
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("DescriptionLength500");
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("Assignmenttypeid")).IsInEnum().WithMessage(context => stringLocalizer.GetString("InvalidFileType"));
            RuleFor(x => x.Answers).NotNull().WithMessage(context => stringLocalizer.GetString("OptionIsRequired"))
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
            RuleFor(x => x.Answers).Must(x => x.Count >= 2).WithMessage(context => stringLocalizer.GetString("OptionMoreThanOne"))
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
            RuleFor(x => x.Answers).Must(a => a.Any(b => b.IsCorrect)).WithMessage(context => stringLocalizer.GetString("OneOptionShouldBeCreated"))
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
            RuleForEach(x => x.Answers).SetValidator(new AssignmentQuestionOptionValidator(stringLocalizer))
                                    .When(x => x.Type == QuestionTypeEnum.SingleChoice || x.Type == QuestionTypeEnum.MultipleChoice);
        }
    }

    public class AssignmentQuestionOptionValidator : AbstractValidator<AssignmentQuestionOptionRequestModel>
    {
        public AssignmentQuestionOptionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Option).MaximumLength(5000).WithMessage(context => stringLocalizer.GetString("OptionLengthMustBe5000"));
            RuleFor(x => x.Option).NotEmpty().NotNull().WithMessage(context => stringLocalizer.GetString("AssignementQuestionisRequired"));
        }
    }
}
