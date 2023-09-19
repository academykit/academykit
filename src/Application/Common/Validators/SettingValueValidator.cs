using System.Text;
using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.ValidatorLocalization;
using Microsoft.Extensions.Localization;

namespace Lingtren.Application.Common.Validators
{
    public class SettingValueValidator : AbstractValidator<SettingValue>
    {
        public SettingValueValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("ValueCannotBeNullOrEmpty"));
            RuleFor(x => x.Key).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("KeyCannotBeNull"));
        }
    }
}
