using Lingtren.Application.ValidatorLocalization;
using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Microsoft.Extensions.Localization;

namespace Lingtren.Application.Common.Validators
{
    public class CertificateRequestModelValidator: AbstractValidator<CertificateRequestModel>
    {
        public CertificateRequestModelValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.StartDate).NotNull().NotEmpty().WithMessage(stringLocalizer.GetString("StartdateEmptyError"));
            RuleFor(x => x.EndDate).NotNull().NotEmpty().WithMessage(stringLocalizer.GetString("EndTimeEmptyError"));
        }
    }
}
