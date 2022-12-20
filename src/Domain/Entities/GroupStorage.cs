namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class GroupStorage : AuditableEntity
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public User User { get; set; }
    }
}