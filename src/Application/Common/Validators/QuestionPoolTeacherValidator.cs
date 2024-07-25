namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class QuestionPoolTeacherValidator : AbstractValidator<QuestionPoolTeacherRequestModel>
    {
        public QuestionPoolTeacherValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.QuestionPoolIdentity)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("QuestionpoolIdRequired"));
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("EmailRequired"));
        }
    }
}
