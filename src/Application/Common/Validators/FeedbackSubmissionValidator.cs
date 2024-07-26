namespace AcademyKit.Application.Common.Validators
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.ValidatorLocalization;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class FeedbackSubmissionValidator
        : AbstractValidator<IList<FeedbackSubmissionRequestModel>>
    {
        public FeedbackSubmissionValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleForEach(x => x)
                .SetValidator(new SingleFeedbackSubmissionValidator(stringLocalizer));
        }
    }

    public class SingleFeedbackSubmissionValidator
        : AbstractValidator<FeedbackSubmissionRequestModel>
    {
        public SingleFeedbackSubmissionValidator(
            IStringLocalizer<ValidatorLocalizer> stringLocalizer
        )
        {
            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrEmpty(x.Answer) || x.SelectedOption.Count > 0 || x.Rating > 0
                )
                .WithMessage(context => stringLocalizer.GetString("FeedbackRequired"));
        }
    }
}
