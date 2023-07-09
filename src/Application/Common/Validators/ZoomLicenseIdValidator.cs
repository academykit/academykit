using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.ValidatorLocalization;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System.Globalization;

namespace Lingtren.Application.Common.Validators
{
    public class ZoomLicenseIdValidator : AbstractValidator<LiveClassLicenseRequestModel>
    {
        public ZoomLicenseIdValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.StartDateTime).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("EventStartDateRequired")).Must(BeValidDateFormat).WithMessage(context => stringLocalizer.GetString("StartDateFormat"));
            RuleFor(x => x.Duration).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("DurationRequired"));


        }
        public bool BeValidDateFormat(DateTime startDateTime)
        {
            if (startDateTime == null)
                return true; // Ignore null values since NotNull() rule is already applied

            // Check if the format is valid
            string format = "yyyy-MM-dd HH:mm:ss";
            string formattedDateTime = startDateTime.ToString(format);
            return startDateTime.ToString(format) == formattedDateTime;
        }
    }  
}
