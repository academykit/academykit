namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;

    public class FeedbackSubmissionValidator : AbstractValidator<IList<FeedbackSubmissionRequestModel>>
    {
        public FeedbackSubmissionValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleFeedbackSubmissionValidator());
        }
    }
    public class SingleFeedbackSubmissionValidator : AbstractValidator<FeedbackSubmissionRequestModel>
    {
        public SingleFeedbackSubmissionValidator()
        {
            RuleFor(x => x).Must(x => !string.IsNullOrEmpty(x.Answer) || x.SelectedOption.Count > 0 || x.Rating > 0)
                .WithMessage("Please provide feedback for all questions.");
        }
    }
}
