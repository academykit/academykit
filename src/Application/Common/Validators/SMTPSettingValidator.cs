namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class SMTPSettingValidator : AbstractValidator<SMTPSettingRequestModel>
    {
        public SMTPSettingValidator()
        {
            RuleFor(x => x.MailServer).NotNull().NotEmpty().WithMessage("Mail server is required").MaximumLength(200).WithMessage("Mail server length must be less than or equal to 200 characters");
            RuleFor(x => x.MailPort).NotNull().NotEmpty().WithMessage("Mail port is required");
            RuleFor(x => x.SenderName).NotNull().NotEmpty().WithMessage("Sender name is required").MaximumLength(200).WithMessage("Sender name length must be less than or equal to 200 characters");
            RuleFor(x => x.SenderEmail).NotNull().NotEmpty().WithMessage("Sender email is required").MaximumLength(200).WithMessage("Sender email length must be less than or equal to 200 characters");
            RuleFor(x => x.UserName).NotNull().NotEmpty().WithMessage("User name is required").MaximumLength(100).WithMessage("User name length must be less than or equal to 100 characters");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required").MaximumLength(100).WithMessage("Password length must be less than or equal to 100 characters");
            RuleFor(x => x.ReplyTo).NotNull().NotEmpty().WithMessage("Reply to is required").MaximumLength(200).WithMessage("Reply to length must be less than or equal to 200 characters");
        }
    }
}
