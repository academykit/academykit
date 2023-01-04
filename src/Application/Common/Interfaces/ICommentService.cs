namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using System;
    using System.Threading.Tasks;

    public interface ICommentService : IGenericService<Comment, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to get comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated items</returns>
        Task<SearchResult<CommentResponseModel>> SearchAsync(string identity, BaseSearchCriteria criteria);

        /// <summary>
        /// Handle to create comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CommentResponseModel"/></returns>
        Task<CommentResponseModel> CreateAsync(string identity, CommentRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to create comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="commentId">the comment id</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CommentResponseModel"/></returns>
        Task<CommentResponseModel> UpdateAsync(string identity, Guid commentId, CommentRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to delete comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="id">the comment id</param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns>the task complete</returns>
        Task DeleteAsync(string identity, Guid id, Guid currentUserId);

        /// <summary>
        /// Handle to search reply
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="id">the comment id</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated result</returns>
        Task<SearchResult<CommentReplyResponseModel>> SearchReplyAsync(string identity, Guid id, BaseSearchCriteria criteria);

        /// <summary>
        /// Handle to create comment reply
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="commentId">the comment id</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        Task<CommentReplyResponseModel> CreateReplyAsync(string identity, Guid commentId, CommentRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to create comment reply
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="commentId">the comment id</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns>the instance of <see cref="CommentReplyResponseModel"/></returns>
        Task<CommentReplyResponseModel> UpdateReplyAsync(string identity, Guid commentId, Guid replyId, CommentRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to delete comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="id">the comment id</param>
        /// <param name="replyId">the comment id</param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns>the task complete</returns>
        Task DeleteReplyAsync(string identity, Guid id, Guid replyId, Guid currentUserId);
    }
}
