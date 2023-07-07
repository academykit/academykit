using FluentValidation;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtring.Application.Common.Validators;
namespace lingtrin.Application.Common.Validators
{
    public class StorageSettingRequestModelValidation : AbstractValidator<StorageSettingRequestModel>
    {
        public StorageSettingRequestModelValidation()
        {
            RuleFor(x => x.Values).Must(x=>x.Any(y=> y.Value != null)).NotNull().NotEmpty().WithMessage("Value type cannot be null");
            RuleFor(x => x.Values.Select(x => x.Key != null)).NotNull().NotEmpty().WithMessage("Value type cannot be null");
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("Value type cannot be null");
            RuleForEach(x => x.Values).SetValidator(new SettingValueValidator());
        }
    }
}
