namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotNull().NotEmpty().WithMessage("Current password is required.");
            RuleFor(x => x.NewPassword).NotNull().NotEmpty().WithMessage("New password is required").MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).NotNull().NotEmpty().WithMessage("Confirm password is required");
            RuleFor(x => x.NewPassword).Equal(x=>x.ConfirmPassword).WithMessage("New password and Confirm password does not matched");
        }
    }
}
