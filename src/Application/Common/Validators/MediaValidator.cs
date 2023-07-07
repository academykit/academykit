using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.ValidatorLocalization;
using Microsoft.Extensions.Localization;

namespace Lingtren.Application.Common.Validators
{
    public class MediaValidator : AbstractValidator<MediaRequestModel>
    {
        public MediaValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.File).NotEmpty().NotNull().WithMessage(context => stringLocalizer.GetString("SelectFileType"));
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage(context => stringLocalizer.GetString("MediaTypeRequired")).IsInEnum().WithMessage(context => stringLocalizer.GetString("InvalidMediaType"));
        }
    }
}
