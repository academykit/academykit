namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
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
