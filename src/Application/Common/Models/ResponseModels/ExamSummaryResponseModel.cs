namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Enums;

    public class ExamSummaryResponseModel
    {
        public Guid LessonId { get; set; }
        public string LessonName { get; set; }
        public CourseStatus LessonStatus { get; set; }
        public ExamStatus ExamStatus { get; set; }
        public IList<MostWrongAnsQues> MostWrongAnsQues { get; set; }
        public IList<UserModel> WeekStudents { get; set; }
        public IList<UserModel> TopStudents { get; set; }
        public IList<TotalMarks> TotalMarks {get; set;}

        public ExamSummaryResponseModel() { }
    }

    public class ExamStatus
    {
        public int TotalAttend { get; set; }
        public int PassStudents { get; set; }
        public int FailStudents { get; set; }
        public decimal AverageMarks { get; set; }
    }

    public class MostWrongAnsQues
    {
        public string Name { get; set; }
    }

    public class TotalMarks
    {
        public decimal Marks {get; set;}
    }
}

