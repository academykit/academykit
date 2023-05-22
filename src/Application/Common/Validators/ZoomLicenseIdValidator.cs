using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System.Globalization;

namespace Lingtren.Application.Common.Validators
{
    public class ZoomLicenseIdValidator : AbstractValidator<LiveClassLicenseRequestModel>
    {
        public ZoomLicenseIdValidator()
        {
            RuleFor(x => x.StartDateTime).NotNull().NotEmpty().WithMessage("Startdate is required").Must(BeValidDateFormat).WithMessage("startdate must be in format YYYY-MM-DD ");
            RuleFor(x => x.Duration).NotNull().NotEmpty().WithMessage("Duration is required");


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
