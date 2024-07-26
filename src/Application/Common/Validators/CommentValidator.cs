namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class CommentValidator : AbstractValidator<CommentRequestModel>
    {
        public CommentValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Content)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ContentRequired"))
                .MaximumLength(500)
                .WithMessage(context => stringLocalizer.GetString("NameLength500"));
        }
    }
}
