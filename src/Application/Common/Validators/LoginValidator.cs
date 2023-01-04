namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;

    public class LoginValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required");
        }
    }
}
