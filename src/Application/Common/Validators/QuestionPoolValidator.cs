namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class QuestionPoolValidator : AbstractValidator<QuestionPoolRequestModel>
    {
        public QuestionPoolValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Question pool name is required");
        }
    }
}
