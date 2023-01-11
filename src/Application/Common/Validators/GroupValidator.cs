namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class GroupValidator : AbstractValidator<GroupRequestModel>
    {
        public GroupValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required.").MaximumLength(250).WithMessage("Name length must be less than or equal to 250 characters.");
        }
    }
}
