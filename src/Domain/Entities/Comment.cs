﻿namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

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
