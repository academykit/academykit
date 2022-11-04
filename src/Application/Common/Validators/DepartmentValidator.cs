namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class DepartmentValidator : AbstractValidator<DepartmentRequestModel>
    {
        public DepartmentValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Department name is required");
        }
    }
}
