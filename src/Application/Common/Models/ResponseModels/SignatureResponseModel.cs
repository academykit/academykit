namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class SignatureResponseModel
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Designation { get; set; }
        public string FullName { get; set; }
        public string SignatureURL { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
