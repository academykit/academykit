namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.ValidatorLocalization;
    using Lingtren.Domain.Enums;
    using Microsoft.Extensions.Localization;

    public class LessonValidator : AbstractValidator<LessonRequestModel>
    {
        public LessonValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.SectionIdentity)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("SectionIdRequired"));
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .When(x => x.Type != LessonType.Exam)
                .WithMessage(context => stringLocalizer.GetString("LessionNameRequired"))
                .MaximumLength(250)
                .WithMessage(context => stringLocalizer.GetString("NameLength250"));
            RuleFor(x => x.Type)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("LessonIdRequired"))
                .IsInEnum()
                .WithMessage(context => stringLocalizer.GetString("InvalidLessionType"));
            RuleFor(x => x.DocumentUrl)
                .NotNull()
                .NotEmpty()
                .When(x => x.Type == LessonType.Document)
                .WithMessage(context => stringLocalizer.GetString("DocumentTypeRequired"));
            RuleFor(x => x.VideoUrl)
                .NotNull()
                .NotEmpty()
                .When(x => x.Type == LessonType.Video || x.Type == LessonType.RecordedVideo)
                .WithMessage(context => stringLocalizer.GetString("VideoIsRequired"));
            RuleFor(x => x.Description)
                .MaximumLength(5000)
                .WithMessage(context => stringLocalizer.GetString("DescriptionLength500"));
            RuleFor(x => x.QuestionSet)
                .SetValidator(new QuestionSetValidator(stringLocalizer))
                .When(x => x.Type == LessonType.Exam);
            RuleFor(x => x.Meeting)
                .SetValidator(new MeetingValidator(stringLocalizer))
                .When(x => x.Type == LessonType.LiveClass);
            RuleFor(x => x)
                .SetValidator(new AssignmentDateValidator(stringLocalizer))
                .When(x => x.Type == LessonType.Assignment);
        }
    }

    public class AssignmentDateValidator : AbstractValidator<LessonRequestModel>
    {
        public AssignmentDateValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x)
                .Must(x =>
                    x.EndDate != default && x.StartDate != default && x.EndDate > x.StartDate
                )
                .WithMessage(context => stringLocalizer.GetString("EnddateMustBeGreater"));
        }
    }

    public class QuestionSetValidator : AbstractValidator<QuestionSetRequestModel>
    {
        public QuestionSetValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.QuestionMarking)
                .NotNull()
                .NotEmpty()
                .WithMessage(context => stringLocalizer.GetString("QuestionMarkingRequired"));
            RuleFor(x => x.AllowedRetake)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(context => stringLocalizer.GetString("RetakeMustBeGreaterThan0"));
            RuleFor(x => x.Description)
                .MaximumLength(5000)
                .WithMessage(context => stringLocalizer.GetString("DescriptionLength500"));
            RuleFor(x => x)
                .Must(x =>
                    x.EndTime != default && x.StartTime != default && x.EndTime > x.StartTime
                )
                .WithMessage(context => stringLocalizer.GetString("EnddateMustBeGreater"));
        }
    }

    public class MeetingValidator : AbstractValidator<MeetingRequestModel>
    {
        public MeetingValidator(IStringLocalizer<ValidatorLocalizer> stringLocalizer)
        {
            RuleFor(x => x.ZoomLicenseId)
                .NotNull()
                .NotEmpty()
                .WithMessage(stringLocalizer.GetString("ZoomLicenseRequired"));
            RuleFor(x => x.MeetingStartDate)
                .NotNull()
                .NotEmpty()
                .WithMessage(stringLocalizer.GetString("EventStartDateRequired"));
            RuleFor(x => x.MeetingDuration)
                .NotNull()
                .NotEmpty()
                .WithMessage(stringLocalizer.GetString("MeedingDurationRequired"))
                .LessThanOrEqualTo(1439)
                .WithMessage(stringLocalizer.GetString("MeetingIdDUrationLength"));
        }
    }
}
