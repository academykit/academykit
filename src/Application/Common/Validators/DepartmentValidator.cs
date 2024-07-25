namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class DepartmentValidator : AbstractValidator<DepartmentRequestModel>
    {
        public DepartmentValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("DepartmentIdRequired"))
                .MaximumLength(250)
                .WithMessage(context => stringLocalizer.GetString("NameLength250"));
        }
    }
}
