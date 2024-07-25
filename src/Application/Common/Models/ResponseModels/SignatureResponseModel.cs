namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class SignatureResponseModel
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Designation { get; set; }
        public string FullName { get; set; }
        public string FileUrl { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public SignatureResponseModel(Signature model)
        {
            Id = model.Id;
            CourseId = model.CourseId;
            Designation = model.Designation;
            FullName = model.FullName;
            FileUrl = model.FileUrl;
            UpdatedOn = model.UpdatedOn ?? model.CreatedOn;
        }
    }
}
