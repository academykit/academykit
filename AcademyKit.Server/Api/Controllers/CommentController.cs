namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Infrastructure.Localization;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/course/{identity}/comments")]
    public class CommentController : BaseApiController
    {
        private readonly ICommentService commentService;
        private readonly IValidator<CommentRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public CommentController(
            ICommentService commentService,
            IValidator<CommentRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.commentService = commentService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// get comment api.
        /// </summary>
        /// <returns> the list of <see cref="CommentResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<CommentResponseModel>> SearchAsync(
            string identity,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await commentService.SearchAsync(identity, searchCriteria).ConfigureAwait(false);
        }

        /// <summary>
        /// create department api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="DepartmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="DepartmentRequestModel" /> .</returns>
        [HttpPost]
        public async Task<CommentResponseModel> CreateAsync(
            string identity,
            CommentRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await commentService
                .CreateAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// create department api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="DepartmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="DepartmentRequestModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<CommentResponseModel> UpdateAsync(
            string identity,
            Guid id,
            CommentRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await commentService
                .UpdateAsync(identity, id, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// delete department api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string identity, Guid id)
        {
            await commentService.DeleteAsync(identity, id, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("CommentRemoved")
                }
            );
        }

        /// <summary>
        /// get comment api.
        /// </summary>
        /// <returns> the list of <see cref="CommentResponseModel" /> .</returns>
        [HttpGet("{id}")]
        public async Task<SearchResult<CommentReplyResponseModel>> SearchReplyAsync(
            string identity,
            Guid id,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await commentService
                .SearchReplyAsync(identity, id, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// get department by id or slug.
        /// </summary>
        /// <param name="identity"> the department id or slug.</param>
        /// <returns> the instance of <see cref="DepartmentResponseModel" /> .</returns>
        [HttpPost("{id}/CommentReply")]
        public async Task<CommentReplyResponseModel> CreateReplyAsync(
            string identity,
            Guid id,
            CommentRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await commentService
                .CreateReplyAsync(identity, id, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// create department api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="DepartmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="DepartmentRequestModel" /> .</returns>
        [HttpPut("{id}/CommentReply/{replyId}")]
        public async Task<CommentReplyResponseModel> UpdateReplyAsync(
            string identity,
            Guid id,
            Guid replyId,
            CommentRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await commentService
                .UpdateReplyAsync(identity, id, replyId, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// change department status api.
        /// </summary>
        /// <param name="identity">the department id or slug.</param>
        /// <param name="enabled">the boolean.</param>
        /// <returns>the instance of <see cref="DepartmentResponseModel"/>.</returns>
        [HttpDelete("{id}/CommentReply/{replyId}")]
        public async Task<IActionResult> DeleteReplyAsync(string identity, Guid id, Guid replyId)
        {
            await commentService
                .DeleteReplyAsync(identity, id, replyId, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("CommentReplyRemoved")
                }
            );
        }
    }
}
