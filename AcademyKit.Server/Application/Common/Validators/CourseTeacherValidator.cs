namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class CourseTeacherValidator : AbstractValidator<CourseTeacherRequestModel>
    {
        public CourseTeacherValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.CourseIdentity)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("TrainingIdRequired"));
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("EmailRequired"));
        }
    }
}
