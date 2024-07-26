namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class QuestionPoolValidator : AbstractValidator<QuestionPoolRequestModel>
    {
        public QuestionPoolValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("QuestionPoolNameRequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("NameLengthError"));
        }
    }
}
