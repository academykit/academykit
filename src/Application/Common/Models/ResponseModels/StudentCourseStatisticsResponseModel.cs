namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class StudentCourseStatisticsResponseModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public double Percentage { get; set; }
        public Guid? LessonId { get; set; }
        public string LessonSlug { get; set; }
        public string LessonName { get; set; }
        public bool? HasCertificateIssued { get; set; }
        public string CertificateUrl { get; set; }
        public DateTime? CertificateIssuedDate { get; set; }
    }
}
