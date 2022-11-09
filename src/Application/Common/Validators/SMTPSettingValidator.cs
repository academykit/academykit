namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class SMTPSettingValidator : AbstractValidator<SMTPSettingRequestModel>
    {
        public SMTPSettingValidator()
        {
            RuleFor(x => x.MailServer).NotNull().NotEmpty().WithMessage("Mail server is required");
            RuleFor(x => x.MailPort).NotNull().NotEmpty().WithMessage("Mail port is required");
            RuleFor(x => x.SenderName).NotNull().NotEmpty().WithMessage("Sender name is required");
            RuleFor(x => x.SenderEmail).NotNull().NotEmpty().WithMessage("Sender email is required");
            RuleFor(x => x.UserName).NotNull().NotEmpty().WithMessage("User name is required");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.ReplayTo).NotNull().NotEmpty().WithMessage("Reply to is required");
            RuleFor(x => x.UseSSL).NotNull().WithMessage("Use SSL is required");
        }
    }
}
