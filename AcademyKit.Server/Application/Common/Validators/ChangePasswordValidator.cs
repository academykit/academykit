﻿namespace AcademyKit.Application.Common.Validators
{
    using System.Text.RegularExpressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.ValidatorLocalization;
    using AcademyKit.Server.Application.Common.Validators;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestModel>
    {
        public ChangePasswordValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.NewPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("NewPasswordRequired"))
                .Length(6, 20)
                .Must(pw => ValidationHelpers.HasValidPassword(pw))
                .WithMessage("InvalidPasswordFormat");
            RuleFor(x => x.ConfirmPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("ConformPassword"));
            RuleFor(x => x.NewPassword)
                .Equal(x => x.ConfirmPassword)
                .WithMessage(context => stringLocalizer.GetString("OldAndNewPasswordDoesNotMatch"));
        }
    }
}
