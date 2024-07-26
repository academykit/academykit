using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.ValidatorLocalization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Application.Common.Validators
{
    public class MediaValidator : AbstractValidator<MediaRequestModel>
    {
        public MediaValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.File)
                .NotEmpty()
                .NotNull()
                .WithMessage(context => stringLocalizer.GetString("SelectFileType"));
            RuleFor(x => x.Type)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("MediaTypeRequired"))
                .IsInEnum()
                .WithMessage(context => stringLocalizer.GetString("InvalidMediaType"));
        }
    }
}
