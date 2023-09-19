namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Lingtren.Domain.Enums;
    using Microsoft.Extensions.Localization;

    public class CourseStatusValidator : AbstractValidator<CourseStatusRequestModel>
    {
        public CourseStatusValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.Identity).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("CourseIdRequired"));
            RuleFor(x => x.Status).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("CourseStatusRequired"));
            RuleFor(x => x.Message).NotNull().NotEmpty().When(x => x.Status == CourseStatus.Rejected).WithMessage(context => stringLocalizer.GetString("RejectMessageRequired"));
        }
    }
}
