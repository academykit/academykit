﻿namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class CommentReply : AuditableEntity
    {
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; }
    }
}
