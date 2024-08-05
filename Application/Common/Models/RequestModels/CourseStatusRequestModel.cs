namespace AcademyKit.Application.Common.Models.RequestModels
{
    using AcademyKit.Domain.Enums;

    public class CourseStatusRequestModel
    {
        public string Identity { get; set; }
        public CourseStatus Status { get; set; }
        public string Message { get; set; }
    }
}
