using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.ValidatorLocalization;
using Microsoft.Extensions.Localization;

namespace Lingtren.Application.Common.Validators
{
    public class ZoomLicenseIdValidator : AbstractValidator<LiveClassLicenseRequestModel>
    {
        public ZoomLicenseIdValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.StartDateTime).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("EventStartDateRequired")).Must(BeValidDateFormat).WithMessage(context => stringLocalizer.GetString("StartDateFormat"));
            RuleFor(x => x.Duration).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("DurationRequired"));
        }

        public static bool BeValidDateFormat(DateTime startDateTime)
        {
            // Check if the format is valid
            var format = "yyyy-MM-dd HH:mm:ss";
            var formattedDateTime = startDateTime.ToString(format);
            return startDateTime.ToString(format) == formattedDateTime;
        }
    }
}
