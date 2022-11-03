namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class UserValidator : AbstractValidator<UserRequestModel>
    {
        public UserValidator()
        {
            RuleSet("Add", () =>
            {
                RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First name is required");
                RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last name is required");
                RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required");
                RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage("Role is required");
            });

            RuleSet("Update", () =>
            {
                RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First name is required");
                RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last name is required");
                RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required");
                RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage("Role is required");
            });
        }
    }
}
