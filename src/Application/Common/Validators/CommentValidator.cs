namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    public class CommentValidator : AbstractValidator<CommentRequestModel>
    {
        public CommentValidator()
        {
            RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("Content is required.")
                .MaximumLength(500).WithMessage("Name length must be less than or equal to 500 characters.");
        }
    }
}
