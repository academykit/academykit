﻿using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators
{
    public class StorageSettingRequestModelValidation
        : AbstractValidator<StorageSettingRequestModel>
    {
        public StorageSettingRequestModelValidation(
            IStringLocalizer<ValidatorLocalizer> stringLocalizer
        )
        {
            RuleFor(x => x.Values)
                .Must(x => x.Any(y => y.Value != null))
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ValueCannotBeNullOrEmpty"));
            RuleFor(x => x.Values.Select(x => x.Key != null))
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ValueCannotBeNullOrEmpty"));
            RuleFor(x => x.Type)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ValueCannotBeNullOrEmpty"));
            RuleForEach(x => x.Values).SetValidator(new SettingValueValidator(stringLocalizer));
        }
    }
}
