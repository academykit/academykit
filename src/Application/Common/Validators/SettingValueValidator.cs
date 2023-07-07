using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtring.Application.Common.Validators
{
    public class SettingValueValidator:AbstractValidator<SettingValue>
    {
        public SettingValueValidator()
        {
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage("Value cannot be null or empty");
            RuleFor(x => x.Key).NotNull().NotEmpty().WithMessage("Key cannot be null");
        }
    }
}
