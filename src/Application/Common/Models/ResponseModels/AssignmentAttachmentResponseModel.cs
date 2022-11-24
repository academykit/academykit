namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class AssignmentAttachmentResponseModel
    {
        public Guid AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string FileUrl { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public UserModel User { get; set; }
        public AssignmentAttachmentResponseModel(AssignmentAttachment attachment)
        {
            AssignmentId= attachment.AssignmentId;
            AssignmentName = attachment.Assignment?.Name;
            FileUrl= attachment.FileUrl;
            Order = attachment.Order;
            Name = attachment.Name;
            MimeType = attachment.MimeType;
            User = attachment.User != null ? new UserModel(attachment.User) : new UserModel();
        }
    }
}
