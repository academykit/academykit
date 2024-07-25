namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class CommentResponseModel
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public int RepliesCount { get; set; }
        public UserModel User { get; set; }
    }

    public class CommentReplyResponseModel
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public UserModel User { get; set; }
    }
}
