namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class CommentValidator : AbstractValidator<CommentRequestModel>
    {
        public CommentValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage(context=> stringLocalizer.GetString("ContentRequired"))
                .MaximumLength(500).WithMessage(context => stringLocalizer.GetString("NameLength500"));
        }
    }
}
