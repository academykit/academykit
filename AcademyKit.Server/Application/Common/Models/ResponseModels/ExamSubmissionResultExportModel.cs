namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class ExamSubmissionResultExportModel
    {
        public string StudentName { get; set; }
        public decimal TotalMarks { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }
}
