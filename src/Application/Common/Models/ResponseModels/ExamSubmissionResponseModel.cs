namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class ExamSubmissionResponseModel
    {
        public UserModel Student { get; set; }
        public decimal TotalMarks { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }
}
