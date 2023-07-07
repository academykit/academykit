namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class SMTPSettingValidator : AbstractValidator<SMTPSettingRequestModel>
    {
        public SMTPSettingValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.MailServer).NotNull().NotEmpty().WithMessage(context =>stringLocalizer.GetString("MailServerRequired")).MaximumLength(200).WithMessage(context => stringLocalizer.GetString("MailServerLength200"));
            RuleFor(x => x.MailPort).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("MailPortRequired"));
            RuleFor(x => x.SenderName).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("SenderNameRequired")).MaximumLength(200).WithMessage(context => stringLocalizer.GetString("SenderName200"));
            RuleFor(x => x.SenderEmail).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("SenderEmailRequired")).MaximumLength(200).WithMessage(context => stringLocalizer.GetString("SenderEmailLength200"));
            RuleFor(x => x.UserName).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("UserNameRequired")).MaximumLength(100).WithMessage(context => stringLocalizer.GetString("UserNameLength100"));
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("PasswordRequired")).MaximumLength(100).WithMessage(context => stringLocalizer.GetString("PasswordLength100"));
            RuleFor(x => x.ReplyTo).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("ReplyToRequired")).MaximumLength(200).WithMessage(context => stringLocalizer.GetString("ReplyToLength200"));
        }
    }
}
