namespace Lingtren.Application.Common.Models.RequestModels
{
    using Lingtren.Domain.Enums;

    public class CourseStatusRequestModel
    {
        public string Identity { get; set; }
        public CourseStatus Status { get; set; }
        public string Message { get; set; }
    }
}
