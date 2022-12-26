using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;

namespace Lingtren.Application.Common.Validators
{
    public class SignatureValidator : AbstractValidator<SignatureRequestModel>
    {
        public SignatureValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().NotNull().WithMessage("Full name is required").MaximumLength(50).WithMessage("Full name length should be less than or equal to 500");
            RuleFor(x => x.Designation).NotEmpty().NotNull().WithMessage("Full name is required").MaximumLength(50).WithMessage("Designation length should be less than or equal to 50");
            RuleFor(x => x.FileURL).NotEmpty().NotNull().WithMessage("File url is required").MaximumLength(200).WithMessage("File url length should be less than or equal to 200");
        }
    }
}
