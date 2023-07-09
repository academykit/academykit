namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Lingtren.Domain.Enums;
    using Microsoft.Extensions.Localization;

    public class QuestionValidator : AbstractValidator<QuestionRequestModel>
    {
        public QuestionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage(context => stringLocalizer.GetString("QuestionPoolNameRequired")).MaximumLength(500)
                        .WithMessage(context => stringLocalizer.GetString("NameLength500"));
            RuleFor(x => x.Hints).MaximumLength(5000).WithMessage(context => stringLocalizer.GetString("QuestionHint5000Length"));
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage(context => stringLocalizer.GetString("DescriptionLenght500"));
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("Question type is required.").IsInEnum().WithMessage(context => stringLocalizer.GetString("InvalidQuestionType"));
            RuleFor(x => x.Answers).NotNull().WithMessage(context => stringLocalizer.GetString("OptionIsRequired"));
            RuleFor(x => x.Answers).Must(x => x.Count >= 2).WithMessage(context => stringLocalizer.GetString("OptionMoreThanOne"));
            RuleFor(x => x.Answers).Must(a => a.Any(b => b.IsCorrect)).WithMessage(context => stringLocalizer.GetString("OneOptionShouldBeCreated"));
            RuleFor(x => x.Answers.Count(a => a.IsCorrect)).LessThanOrEqualTo(1).When(x => x.Type == QuestionTypeEnum.SingleChoice)
                                            .WithMessage(context => stringLocalizer.GetString("SelectOneCorrectAnswer"));
            RuleForEach(x => x.Answers).SetValidator(new QuestionOptionValidator(stringLocalizer));
        }
    }

    public class QuestionOptionValidator : AbstractValidator<QuestionOptionRequestModel>
    {
        public QuestionOptionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Option).MaximumLength(5000).WithMessage(context => stringLocalizer.GetString("OptionLengthMustBe5000"));
            RuleFor(x => x.Option).NotEmpty().NotNull().WithMessage(context => stringLocalizer.GetString("OptionIsRequired"));
        }
    }
}
