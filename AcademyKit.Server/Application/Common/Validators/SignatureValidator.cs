using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators
{
    public class SignatureValidator : AbstractValidator<SignatureRequestModel>
    {
        public SignatureValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("FullNameRequired"))
                .MaximumLength(50)
                .WithMessage(context => stringLocalizer.GetString("NameLength500"));
            RuleFor(x => x.Designation)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("FullNameRequired"))
                .MaximumLength(50)
                .WithMessage(context => stringLocalizer.GetString("DesignationLength50"));
            RuleFor(x => x.FileURL)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("FileUrlRequired"))
                .MaximumLength(200)
                .WithMessage(context => stringLocalizer.GetString("FileUrlLength200"));
        }
    }
}
