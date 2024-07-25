namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssignmentSummaryResponseModel
    {
        public IList<UserModel> WeekStudents { get; set; }
        public IList<UserModel> TopStudents { get; set; }
        public AssignmentStatus AssignmentStatus { get; set; }
        public IList<string> MostWrongAnsQues { get; set; }
    }

    public class AssignmentStatus
    {
        public int TotalAttend { get; set; }
        public decimal AverageMarks { get; set; }
        public int TotalPass { get; set; }
        public int TotalFail { get; set; }
    }
}
