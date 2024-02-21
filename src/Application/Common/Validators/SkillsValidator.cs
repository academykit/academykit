using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.ValidatorLocalization;
using Microsoft.Extensions.Localization;

namespace Lingtren.Application.Common.Validators
{
    public class SkillsValidator : AbstractValidator<SkillsRequestModel>
    {
        public SkillsValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.SkillName)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("SkillNameRequired"))
                .MaximumLength(50)
                .WithMessage(context => stringLocalizer.GetString("NameLength500"));
        }
    }
}
