namespace Lingtren.Application.Common.Validators
{

    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class CourseCertificateValidator : AbstractValidator<CourseCertificateRequestModel>
    {
        public CourseCertificateValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Title).NotEmpty().NotNull().WithMessage(stringLocalizer.GetString("UserIDrequired")).MaximumLength(100).WithMessage(stringLocalizer.GetString("TitleLenghtIssue"));
            RuleFor(x => x.EventStartDate).NotEmpty().NotNull().WithMessage(stringLocalizer.GetString("EventStartDateRequired"));
            RuleFor(x => x.EventEndDate).NotEmpty().NotNull().WithMessage(stringLocalizer.GetString("EventEndDate"));
            RuleFor(x => x).Must(x => x.EventEndDate.Date >= x.EventStartDate.Date).WithMessage(stringLocalizer.GetString("timeSpanIssue"));
        }
    }
}
