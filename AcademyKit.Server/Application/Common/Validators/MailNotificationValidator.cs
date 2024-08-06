namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Ganss.Xss;
    using Microsoft.Extensions.Localization;

    public class MailNotificationValidator : AbstractValidator<MailNotificationRequestModel>
    {
        public MailNotificationValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            var htmlSanitizer = new HtmlSanitizer();

            RuleFor(x => x.MailName)
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("MailNameRequired"))
                .MaximumLength(250)
                .WithMessage(context => stringLocalizer.GetString("MailNameLength250"));

            RuleFor(x => x.MailSubject)
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("MailSubjectRequired"))
                .MaximumLength(250)
                .WithMessage(context => stringLocalizer.GetString("MailSubjectLength250"));

            RuleFor(x => x.MailType)
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("MailTypeRequired"));

            RuleFor(x => x.MailMessage)
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("MailMessageRequired"))
                .Must(html => IsHtmlSafe(html, htmlSanitizer))
                .WithMessage(context => stringLocalizer.GetString("InvalidHtml"));
        }

        private static bool IsHtmlSafe(string html, HtmlSanitizer sanitizer)
        {
            var sanitizedHtml = sanitizer.Sanitize(html);
            return sanitizedHtml == html;
        }
    }
}
