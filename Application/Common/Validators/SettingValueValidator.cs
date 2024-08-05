using System.Text;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators
{
    public class SettingValueValidator : AbstractValidator<SettingValue>
    {
        public SettingValueValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Value)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ValueCannotBeNullOrEmpty"));
            RuleFor(x => x.Key)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("KeyCannotBeNull"));
        }
    }
}
