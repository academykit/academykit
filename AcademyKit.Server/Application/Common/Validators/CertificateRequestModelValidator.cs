using System.Text.RegularExpressions;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators
{
    public class CertificateRequestModelValidator : AbstractValidator<CertificateRequestModel>
    {
        public CertificateRequestModelValidator(
            IStringLocalizer<ValidatorLocalizer> stringLocalizer
        )
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .NotEmpty()
                .WithMessage(contex => stringLocalizer.GetString("StartdateEmptyError"))
                .Must(IsValidDate)
                .WithMessage(context => stringLocalizer.GetString("CertificateStartTimeError"));
            RuleFor(x => x.EndDate)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("EndTimeEmptyError"))
                .Must(IsValidDate)
                .WithMessage(context => stringLocalizer.GetString("CertificateEndTimeError"));
            RuleFor(x => x.OptionalCost)
                .Must(cost => IsValidCost(cost))
                .WithMessage(context => stringLocalizer.GetString("CannotBeNegative"));
        }

        public static bool IsValidCost(decimal cost)
        {
            var pattern = "^(0(\\.\\d+)?|[1-9]\\d*(\\.\\d+)?)$";
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
