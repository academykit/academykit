namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class Group : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IList<GroupMember> GroupMembers { get; set; }
        public User User { get; set; }
        public IList<Course> Courses { get; set; }
        public IList<GroupFile> GroupFiles { get; set; }
    }
}