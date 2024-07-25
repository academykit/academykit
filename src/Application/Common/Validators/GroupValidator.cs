namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class GroupValidator : AbstractValidator<GroupRequestModel>
    {
        public GroupValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("NameRequired"))
                .MaximumLength(250)
                .WithMessage(context => stringLocalizer.GetString("NameLengthError"));
        }
    }
}
