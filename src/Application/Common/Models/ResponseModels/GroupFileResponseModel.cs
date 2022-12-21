using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class GroupFileResponseModel
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string Url { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public GroupFileResponseModel()
        {
            
        }

        public GroupFileResponseModel(GroupFile groupFile)
        {
            Id = groupFile.Id;
            GroupId = groupFile.GroupId;
            Name = groupFile.Name;
            MimeType = groupFile.MimeType;
            Url = groupFile.Url;
            CreatedOn = groupFile.CreatedOn;
            UpdatedOn = groupFile.UpdatedOn;
        }
    }
}