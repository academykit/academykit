namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using System.Text.RegularExpressions;

    public class CourseValidator : AbstractValidator<CourseRequestModel>
    {
        public CourseValidator()
        {
            RuleFor(x => x.Name).Must(name => RemoveHtmlTags(name).Length <= 100).NotNull().NotEmpty().WithMessage("Name is required.").MaximumLength(100).WithMessage("Name length must be less than or equal to 100 characters.");
            RuleFor(x => x.LevelId).NotNull().NotEmpty().WithMessage("Level is required.");
            RuleFor(x => x.GroupId).NotNull().NotEmpty().WithMessage("Please select group.");
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("Description length must be less than or equal to 5000 characters.");

        }
        private string RemoveHtmlTags(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            string textonly = Regex.Replace(name, "<.*?>", "");
            return textonly;

        }
    }
}
