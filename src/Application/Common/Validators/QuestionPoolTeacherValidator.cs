namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class QuestionPoolTeacherValidator : AbstractValidator<QuestionPoolTeacherRequestModel>
    {
        public QuestionPoolTeacherValidator()
        {
            RuleFor(x => x.QuestionPoolIdentity).NotNull().NotEmpty().WithMessage("Question pool identity is required");
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required");
        }
    }
}
