namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class Comment : AuditableEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; }
        public IList<CommentReply> CommentReplies { get; set; }
    }
}