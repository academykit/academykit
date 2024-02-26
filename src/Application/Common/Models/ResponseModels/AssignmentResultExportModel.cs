namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssignmentResultExportModel
    {
      public string StudentName { get; set; }
      public decimal TotalMarks {get; set;}
      public DateTime? SubmissionDate { get; set; }
    }   
}