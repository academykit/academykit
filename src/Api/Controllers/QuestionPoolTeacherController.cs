namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class QuestionPoolTeacherController : BaseApiController
    {
        private readonly IQuestionPoolService questionPoolService;
        private readonly IQuestionPoolTeacherService questionPoolTeacherService;
        private readonly IUserService userService;
        private readonly IValidator<QuestionPoolTeacherRequestModel> validator;
        private readonly ILogger<QuestionPoolTeacherController> logger;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public QuestionPoolTeacherController(
            IQuestionPoolService questionPoolService,
            IQuestionPoolTeacherService questionPoolTeacherService,
            IUserService userService,
            IValidator<QuestionPoolTeacherRequestModel> validator,
            ILogger<QuestionPoolTeacherController> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.questionPoolService = questionPoolService;
            this.questionPoolTeacherService = questionPoolTeacherService;
            this.userService = userService;
            this.validator = validator;
            this.logger = logger;
            this.localizer = localizer;
        }

        /// <summary>
        /// Searches the question pool teachers.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet]
        public async Task<SearchResult<QuestionPoolTeacherResponseModel>> Search(
            [FromQuery] QuestionPoolTeacherBaseSearchCriteria criteria
        )
        {
            // question pool id is required
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                criteria.QuestionPoolIdentity,
                nameof(criteria.QuestionPoolIdentity)
            );
            criteria.CurrentUserId = CurrentUser.Id;
            var searchResult = await questionPoolTeacherService
                .SearchAsync(criteria)
                .ConfigureAwait(false);

            var response = new SearchResult<QuestionPoolTeacherResponseModel>
            {
                Items = new List<QuestionPoolTeacherResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                response.Items.Add(new QuestionPoolTeacherResponseModel(p))
            );

            return response;
        }

        /// <summary>
        /// add new teacher.
        /// </summary>
        /// <param name="model"> the instance of <see cref="QuestionPoolTeacherRequestModel" /> .</param>
        /// <returns> the instance of <see cref="QuestionPoolTeacherResponseModel" /> .</returns>
        [HttpPost]
        public async Task<QuestionPoolTeacherResponseModel> Create(
            QuestionPoolTeacherRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);

            var questionPool = await questionPoolService
                .GetByIdOrSlugAsync(model.QuestionPoolIdentity, CurrentUser.Id)
                .ConfigureAwait(false);
            var user = await userService.GetUserByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                logger.LogWarning("User with email not found while adding user in pool creator.");
                throw new EntityNotFoundException(localizer.GetString("UserNotFound"));
            }

            if (user.Role == UserRole.Trainee)
            {
                logger.LogWarning(
                    "User with id: {id} having role: {role} cannot added as exam pool creator with poolId: {poolId}.",
                    user.Id,
                    user.Role,
                    questionPool.Id
                );
                throw new ForbiddenException(localizer.GetString("TraineeRoleNotAllowed"));
            }

            var currentTimeStamp = DateTime.UtcNow;
            var questionPoolTeacher = new QuestionPoolTeacher
            {
                Id = Guid.NewGuid(),
                QuestionPoolId = questionPool.Id,
                UserId = user.Id,
                Role = PoolRole.Author,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
            };

            var response = await questionPoolTeacherService
                .CreateAsync(questionPoolTeacher)
                .ConfigureAwait(false);
            return new QuestionPoolTeacherResponseModel(response);
        }

        /// <summary>
        /// change department status api.
        /// </summary>
        /// <param name="identity">the department id or slug.</param>
        /// <param name="enabled">the boolean.</param>
        /// <returns>the instance of <see cref="QuestionPoolTeacherResponseModel"/>.</returns>
        [HttpPatch("{identity}/status")]
        public async Task<QuestionPoolTeacherResponseModel> ChangeStatus(
            string identity,
            [FromQuery] PoolRole role
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            var statusExists = Enum.IsDefined(typeof(PoolRole), role);
            if (!statusExists)
            {
                logger.LogWarning(
                    "Invalid question pool teacher role requested for role change by the user"
                );
                throw new ForbiddenException(localizer.GetString("InvalidQuestionPoolTeacherRole"));
            }

            var existing = await questionPoolTeacherService
                .GetByIdOrSlugAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);

            existing.Id = existing.Id;
            existing.Role = role;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = DateTime.UtcNow;

            var savedEntity = await questionPoolTeacherService
                .UpdateAsync(existing)
                .ConfigureAwait(false);
            return new QuestionPoolTeacherResponseModel(savedEntity);
        }

        /// <summary>
        /// Deletes the question pool teacher.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await questionPoolTeacherService
                .DeleteAsync(id.ToString(), CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel
                {
                    Success = true,
                    Message = localizer.GetString("QuestionpoolTeacherRemoved")
                }
            );
        }
    }
}
