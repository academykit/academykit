using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.ValidatorLocalization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtring.Application.Common.Validators
{
    public class SettingValueValidator:AbstractValidator<SettingValue>
    {
        public SettingValueValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("ValueCannotBeNullOrEmpty"));
            RuleFor(x => x.Key).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("KeyCannotBeNull"));
        }
    }
}
