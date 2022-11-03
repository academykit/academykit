namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class GroupValidator : AbstractValidator<GroupRequestModel>
    {
        public GroupValidator()
        {
            RuleSet("Add", () =>
            {
                RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required");
            });

            RuleSet("Update", () =>
            {
                RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required");
            });
        }
    }
}
