namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;
    public class LessonValidator : AbstractValidator<LessonRequestModel>
    {
        public LessonValidator()
        {
            RuleFor(x => x.SectionIdentity).NotNull().NotEmpty().WithMessage("Section identity is required.");
            RuleFor(x => x.Name).NotNull().NotEmpty().When(x => x.Type != LessonType.Exam)
                                    .WithMessage("Lesson name is required.").MaximumLength(250).WithMessage("Name length must be less than or equal to 250 characters.");
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("Lesson type is required.").IsInEnum().WithMessage("Invalid lesson type.");
            RuleFor(x => x.DocumentUrl).NotNull().NotEmpty().When(x => x.Type == LessonType.Document)
                                    .WithMessage("Document is required for lesson type is document");
            RuleFor(x => x.VideoUrl).NotNull().NotEmpty().When(x => x.Type == LessonType.Video || x.Type == LessonType.RecordedVideo)
                                    .WithMessage("Video is required for lesson type is video or recorded video.");
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("Name length must be less than or equal to 5000 characters.");
            RuleFor(x => x.QuestionSet).SetValidator(new QuestionSetValidator())
                                         .When(x => x.Type == LessonType.Exam);
            RuleFor(x => x.Meeting).SetValidator(new MeetingValidator())
                                         .When(x => x.Type == LessonType.LiveClass);
            RuleFor(x => x).Must(x => x.EndDate != default && x.StartDate != default && x.EndDate > x.StartDate).WithMessage("EndDate must be greater than StartDate.");
        }
    }

    public class QuestionSetValidator : AbstractValidator<QuestionSetRequestModel>
    {
        public QuestionSetValidator()
        {
            RuleFor(x => x.QuestionMarking).NotNull().NotEmpty().WithMessage("Question marking is required.");
            RuleFor(x => x.Description).MaximumLength(5000).WithMessage("Description length must be less than or equal to 5000 characters.");
            RuleFor(x => x).Must(x => x.EndTime != default && x.StartTime != default && x.EndTime > x.StartTime).WithMessage("EndTime must be greater than startTime.");
        }
    }
    public class MeetingValidator : AbstractValidator<MeetingRequestModel>
    {
        public MeetingValidator()
        {
            RuleFor(x => x.ZoomLicenseId).NotNull().NotEmpty().WithMessage("Please select zoom license.");
            RuleFor(x => x.MeetingStartDate).NotNull().NotEmpty().WithMessage("Meeting start date is required.");
            RuleFor(x => x.MeetingDuration).NotNull().NotEmpty().WithMessage("Meeting duration is required.")
                        .LessThanOrEqualTo(1439).WithMessage("Meeting duration cannot be greater than or equal to 1439 minutes.");
        }
    }
}
