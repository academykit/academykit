namespace Lingtren.Application.Common.Models.RequestModels
{
    using Lingtren.Domain.Enums;

    public class AssessmentStatusRequestModel
    {
        public string Identity { get; set; }
        public AssessmentStatus Status { get; set; }
        public string Message { get; set; }
    }
}
