using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;

namespace Lingtren.Application.Common.Validators
{
    public class CertificateRequestModelValidator: AbstractValidator<CertificateRequestModel>
    {
        public CertificateRequestModelValidator()
        {
            RuleFor(x => x.StartDate).NotNull().NotEmpty().WithMessage("startdate cannot be empty");
            RuleFor(x => x.EndDate).NotNull().NotEmpty().WithMessage("End date cannot be empty");
        }
    }
}
