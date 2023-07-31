using Lingtren.Application.ValidatorLocalization;
using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Microsoft.Extensions.Localization;
using Hangfire.Annotations;

namespace Lingtren.Application.Common.Validators
{
    public class CertificateRequestModelValidator: AbstractValidator<CertificateRequestModel>
    {
        public CertificateRequestModelValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.StartDate).NotNull().NotEmpty().WithMessage(contex => stringLocalizer.GetString("StartdateEmptyError")).Must(y => IsValidDate(y)).WithMessage(context => stringLocalizer.GetString("CertificateStartTimeError"));
            RuleFor(x => x.EndDate).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("EndTimeEmptyError")).Must(y=> IsValidDate(y)).WithMessage(context => stringLocalizer.GetString("CertificateEndTimeError"));
        }
        public static bool IsValidDate(DateTime y)
        {
            if (y.Date > DateTime.UtcNow.Date)
            {
                return false;
            }
            return true;
        }
    }
}
