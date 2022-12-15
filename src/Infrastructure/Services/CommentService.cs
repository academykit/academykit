﻿namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;

    public class CommentService : BaseGenericService<Comment, BaseSearchCriteria>, ICommentService
    {
        public CommentService(IUnitOfWork unitOfWork,
            ILogger<CommentService> logger) : base(unitOfWork, logger)
        {

        }

        /// <summary>
        /// Handle to get comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated items</returns>
        public async Task<SearchResult<CommentResponseModel>> SearchAsync(string identity, BaseSearchCriteria criteria)
        {
            var course = await ValidateAndGetCourse(criteria.CurrentUserId, identity, validateForModify: false).ConfigureAwait(false);
            var predicate = PredicateBuilder.New<Comment>(true);
            predicate = predicate.And(p => p.CourseId == course.Id);

            var query = _unitOfWork.GetRepository<Comment>().GetAll(predicate: predicate, include: src => src.Include(x => x.User));
            if (criteria.SortBy == null)
            {
                criteria.SortBy = nameof(Comment.CreatedOn);
                criteria.SortType = SortType.Descending;
            }
            query = criteria.SortType == SortType.Ascending
                ? query.OrderBy(x => criteria.SortBy)
                : query.OrderByDescending(x => criteria.SortBy);
            var result = query.ToList().ToIPagedList(criteria.Page, criteria.Size);

            var response = new SearchResult<CommentResponseModel>
            {
                Items = new List<CommentResponseModel>(),
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPage = result.TotalPage,
            };

            result.Items.ForEach(p =>
                 response.Items.Add(new CommentResponseModel()
                 {
                     Id = p.Id,
                     CourseId = p.CourseId,
                     Content = p.Content,
                     RepliesCount = _unitOfWork.GetRepository<CommentReply>().Count(predicate: x => x.CommentId == p.Id),
                     User = new UserModel(p.User)
                 })
             );
            return response;
        }

        /// <summary>
        /// Handle to search reply 
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="id">the comment id</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated result</returns>
        public async Task<SearchResult<CommentReplyResponseModel>> SearchReplyAsync(string identity, Guid id, BaseSearchCriteria criteria)
        {
            await ValidateAndGetCourse(criteria.CurrentUserId, identity, validateForModify: false).ConfigureAwait(false);
            var predicate = PredicateBuilder.New<CommentReply>(true);
            predicate = predicate.And(p => p.CommentId == id);

            var query = _unitOfWork.GetRepository<CommentReply>().GetAll(predicate: predicate, include: src => src.Include(x => x.User));
            if (criteria.SortBy == null)
            {
                criteria.SortBy = nameof(CommentReply.CreatedOn);
                criteria.SortType = SortType.Descending;
            }
            query = criteria.SortType == SortType.Ascending
                ? query.OrderBy(x => criteria.SortBy)
                : query.OrderByDescending(x => criteria.SortBy);
            var result = query.ToList().ToIPagedList(criteria.Page, criteria.Size);

            var response = new SearchResult<CommentReplyResponseModel>
            {
                Items = new List<CommentReplyResponseModel>(),
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPage = result.TotalPage,
            };

            result.Items.ForEach(p =>
                 response.Items.Add(new CommentReplyResponseModel()
                 {
                     Id = p.Id,
                     CommentId = p.CommentId,
                     Content = p.Content,
                     User = new UserModel(p.User)
                 })
             );
            return response;
        }

        /// <summary>
        /// Handle to create comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CommentResponseModel"/></returns>
        public async Task<CommentResponseModel> CreateAsync(string identity, CommentRequestModel model, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: false).ConfigureAwait(false);
            var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                predicate: p => p.Id == currentUserId
                ).ConfigureAwait(false);
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                Content = model.Content,
                CreatedBy = currentUserId,
                CreatedOn = DateTime.UtcNow,
            };
            await _unitOfWork.GetRepository<Comment>().InsertAsync(comment).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return new CommentResponseModel
            {
                Id = comment.Id,
                CourseId = course.Id,
                Content = comment.Content,
                User = new UserModel
                {
                    Id = currentUserId,
                    FullName = user?.FullName,
                    Email = user?.Email,
                    ImageUrl = user?.ImageUrl,
                    MobileNumber = user?.MobileNumber
                }
            };
        }

        /// <summary>
        /// Handle to create comment
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="commentId">the comment id</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CommentResponseModel"/></returns>
        public async Task<CommentResponseModel> UpdateAsync(string identity, Guid commentId, CommentRequestModel model, Guid currentUserId)
        {
            var existing = await GetAsync(commentId, currentUserId).ConfigureAwait(false);
            if (existing == null)
            {
                _logger.LogWarning("Comment with id :{id} not found", commentId);
                throw new EntityNotFoundException("Comment not found");
            }
            if (existing.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User with id: {currentUserId} is not authorized user to edit comment with id :{id}", currentUserId, commentId);
                throw new ForbiddenException("Unauthorized user to edit comment");
            }
            existing.Content = model.Content;
            existing.UpdatedBy = currentUserId;
            existing.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<Comment>().Update(existing);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return new CommentResponseModel
            {
                Id = existing.Id,
                CourseId = existing.CourseId,
                Content = existing.Content,
                User = new UserModel
                {
                    Id = currentUserId,
                    FullName = existing.User?.FullName,
                    Email = existing.User?.Email,
                    ImageUrl = existing.User?.ImageUrl,
                    MobileNumber = existing.User?.MobileNumber
                }
            };
        }

        /// <summary>
        /// Handle to create comment reply
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="commentId">the comment id</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns>the instance of <see cref="CommentReplyResponseModel"/></returns>
        public async Task<CommentReplyResponseModel> CreateReplyAsync(string identity, Guid commentId, CommentRequestModel model, Guid currentUserId)
        {
            await ValidateAndGetCourse(currentUserId, identity, validateForModify: false).ConfigureAwait(false);
            var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                predicate: p => p.Id == currentUserId
                ).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with id :{id} not found", currentUserId);
                throw new EntityNotFoundException("User not found");
            }

            var reply = new CommentReply
            {
                Id = Guid.NewGuid(),
                Content = model.Content,
                CommentId = commentId,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = currentUserId,
            };
            await _unitOfWork.GetRepository<CommentReply>().InsertAsync(reply).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return new CommentReplyResponseModel
            {
                Id = reply.Id,
                CommentId = commentId,
                Content = reply.Content,
                User = new UserModel
                {
                    Id = currentUserId,
                    FullName = user?.FullName,
                    Email = user?.Email,
                    ImageUrl = user?.ImageUrl,
                    MobileNumber = user?.MobileNumber
                }
            };
        }

        /// <summary>
        /// Handle to create comment reply
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="commentId">the comment id</param>
        /// <param name="model">the instance of <see cref="CommentRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns>the instance of <see cref="CommentReplyResponseModel"/></returns>
        public async Task<CommentReplyResponseModel> UpdateReplyAsync(string identity, Guid commentId, Guid replyId, CommentRequestModel model, Guid currentUserId)
        {
            await ValidateAndGetCourse(currentUserId, identity, validateForModify: false).ConfigureAwait(false);
            var existing = await _unitOfWork.GetRepository<CommentReply>().GetFirstOrDefaultAsync(
                predicate: p => p.Id == replyId && p.CommentId == commentId,
                include: src => src.Include(x => x.User)
                ).ConfigureAwait(false);
            if (existing == null)
            {
                _logger.LogWarning("Comment reply with id :{id} and comment with id :{commentId} not found", replyId, commentId);
                throw new EntityNotFoundException("Comment reply not found");
            }
            if (existing.CreatedBy != currentUserId)
            {
                _logger.LogWarning("User with id: {currentUserId} is not authorized user to edit reply with id: {replyId} comment with id :{id}",
                    currentUserId, replyId, commentId);
                throw new ForbiddenException("Unauthorized user to edit comment");
            }
            var reply = new CommentReply
            {
                Id = Guid.NewGuid(),
                Content = model.Content,
                CommentId = commentId,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = currentUserId,
            };
            await _unitOfWork.GetRepository<CommentReply>().InsertAsync(reply).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return new CommentReplyResponseModel
            {
                Id = reply.Id,
                CommentId = commentId,
                Content = reply.Content,
                User = new UserModel
                {
                    Id = currentUserId,
                    FullName = existing.User?.FullName,
                    Email = existing.User?.Email,
                    ImageUrl = existing.User?.ImageUrl,
                    MobileNumber = existing.User?.MobileNumber
                }
            };
        }

        #region Protected Methods

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Comment, bool>> ConstructQueryConditions(Expression<Func<Comment, bool>> predicate, BaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Content.ToLower().Trim().Contains(search));
            }
            return predicate;
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(BaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Comment.CreatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Comment, object> IncludeNavigationProperties(IQueryable<Comment> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Comment, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }
        #endregion Protected Methods
    }
}
