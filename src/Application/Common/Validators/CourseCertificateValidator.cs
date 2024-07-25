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
            RuleFor(x => x.Title)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("UserIDrequired"))
                .MaximumLength(100)
                .WithMessage(context => stringLocalizer.GetString("TitleLenghtIssue"));
            RuleFor(x => x.EventStartDate)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("EventStartDateRequired"));
            RuleFor(x => x.EventEndDate)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("EventEndDate"));
            RuleFor(x => x)
                .Must(x => x.EventEndDate.Date >= x.EventStartDate.Date)
                .WithMessage(context => stringLocalizer.GetString("timeSpanIssue"));
        }
    }
}
