namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Microsoft.Extensions.Localization;

    public class FeedbackSubmissionValidator : AbstractValidator<IList<FeedbackSubmissionRequestModel>>
    {
        public FeedbackSubmissionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleForEach(x => x).SetValidator(new SingleFeedbackSubmissionValidator(stringLocalizer));
        }
    }
    public class SingleFeedbackSubmissionValidator : AbstractValidator<FeedbackSubmissionRequestModel>
    {
        public SingleFeedbackSubmissionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x).Must(x => !string.IsNullOrEmpty(x.Answer) || x.SelectedOption.Count > 0 || x.Rating > 0)
                .WithMessage(context => stringLocalizer.GetString("FeedbackRequired"));
        }
    }
}
