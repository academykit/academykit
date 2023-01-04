using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;

namespace Lingtren.Application.Common.Validators
{
    public class MediaValidator : AbstractValidator<MediaRequestModel>
    {
        public MediaValidator()
        {
            RuleFor(x => x.File).NotEmpty().NotNull().WithMessage("Please select the file.");
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("Media type is required").IsInEnum().WithMessage("Invalid media type");
        }
    }
}
