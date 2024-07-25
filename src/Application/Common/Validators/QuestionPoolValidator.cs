namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
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
