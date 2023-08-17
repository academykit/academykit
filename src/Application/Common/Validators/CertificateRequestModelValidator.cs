using Lingtren.Application.ValidatorLocalization;
using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace Lingtren.Application.Common.Validators
{
    public class CertificateRequestModelValidator : AbstractValidator<CertificateRequestModel>
    {
        public CertificateRequestModelValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.StartDate).NotNull().NotEmpty().WithMessage(contex => stringLocalizer.GetString("StartdateEmptyError")).Must(IsValidDate).WithMessage(context => stringLocalizer.GetString("CertificateStartTimeError"));
            RuleFor(x => x.EndDate).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("EndTimeEmptyError")).Must(IsValidDate).WithMessage(context => stringLocalizer.GetString("CertificateEndTimeError"));
            RuleFor(x => x.OptionalCost).Must(cost =>IsValidCost(cost)).WithMessage(context => stringLocalizer.GetString("CannotBeNegative"));
        }

        public static bool IsValidCost(decimal cost)
        {
            string pattern = "^(0(\\.\\d+)?|[1-9]\\d*(\\.\\d+)?)$";
           return Regex.IsMatch(cost.ToString(), pattern);
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
