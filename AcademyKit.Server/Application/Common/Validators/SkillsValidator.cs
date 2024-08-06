using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators
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
