namespace AcademyKit.Application.Common.Models.RequestModels
{
    using AcademyKit.Domain.Enums;

    public class AssessmentStatusRequestModel
    {
        public string Identity { get; set; }
        public AssessmentStatus Status { get; set; }
        public string Message { get; set; }
    }
}
