using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class ExamResultExportSubmissionModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string CourseName { get; set; }
        public string LessonName { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal NegativeMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public string ResultStatus { get; set; }

        public ExamResultExportSubmissionModel(QuestionSetResult examResult)
        {
            Id = examResult.Id;
            UserName = examResult.User.FullName;
            CourseName = examResult.QuestionSet.Lesson.Course.Name;
            LessonName = examResult.QuestionSet.Lesson.Name;
            TotalMarks = examResult.TotalMark;
            NegativeMarks = examResult.NegativeMark;
            ObtainedMarks = examResult.TotalMark - examResult.NegativeMark;
            ResultStatus = examResult
                .QuestionSet.Lesson.Course.WatchHistories.FirstOrDefault(x =>
                    x.UserId == examResult.UserId && x.LessonId == examResult.QuestionSet.Lesson.Id
                )
                .IsPassed
                ? "Passed"
                : "Failed";
        }

        public ExamResultExportSubmissionModel() { }
    }
}
