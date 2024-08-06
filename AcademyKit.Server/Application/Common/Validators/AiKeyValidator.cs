namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class AiKeyValidator : AbstractValidator<AiKeyRequestModel>
    {
        public AiKeyValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer) { }
    }
}
