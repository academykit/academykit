namespace Lingtren.Application.Common.Validators
{
    using FluentValidation;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;
    public class LessonValidator : AbstractValidator<LessonRequestModel>
    {
        public LessonValidator()
        {
            RuleFor(x => x.SectionIdentity).NotNull().NotEmpty().WithMessage("Section identity is required");
            RuleFor(x => x.Name).NotNull().NotEmpty().When(x => x.Type != LessonType.Exam)
                                    .WithMessage("Lesson name is required");
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("Lesson type is required").IsInEnum().WithMessage("Invalid lesson type");
            RuleFor(x => x.DocumentUrl).NotNull().NotEmpty().When(x => x.Type == LessonType.Document)
                                    .WithMessage("Document is required for lesson type is document");
            RuleFor(x => x.VideoUrl).NotNull().NotEmpty().When(x => x.Type == LessonType.Video || x.Type == LessonType.RecordedVideo)
                                    .WithMessage("Video is required for lesson type is video or recorded video");
        }
    }
}
