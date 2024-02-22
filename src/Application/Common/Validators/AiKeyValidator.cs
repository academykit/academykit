namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class AiKeyValidator : AbstractValidator<AiKeyRequestModel>
    {
        public AiKeyValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer) { }
    }
}
