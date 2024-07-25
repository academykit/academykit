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

    public class CourseTeacherController : BaseApiController
    {
        private readonly ICourseTeacherService courseTeacherService;
        private readonly ICourseService courseService;
        private readonly IUserService userService;
        private readonly IValidator<CourseTeacherRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public CourseTeacherController(
            ICourseTeacherService courseTeacherService,
            ICourseService courseService,
            IUserService userService,
            IValidator<CourseTeacherRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.courseTeacherService = courseTeacherService;
            this.courseService = courseService;
            this.userService = userService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// Searches the question pool moderators.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet]
        public async Task<SearchResult<CourseTeacherResponseModel>> Search(
            [FromQuery] CourseTeacherSearchCriteria criteria
        )
        {
            // question pool id is required
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                criteria.CourseIdentity,
                nameof(criteria.CourseIdentity)
            );
            criteria.CurrentUserId = CurrentUser.Id;
            var searchResult = await courseTeacherService
                .SearchAsync(criteria)
                .ConfigureAwait(false);

            var response = new SearchResult<CourseTeacherResponseModel>
            {
                Items = new List<CourseTeacherResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p => response.Items.Add(new CourseTeacherResponseModel(p)));

            return response;
        }

        /// <summary>
        /// add new moderator.
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseTeacherRequestModel" /> .</param>
        /// <returns> the instance of <see cref="CourseTeacherResponseModel" /> .</returns>
        [HttpPost]
        public async Task<CourseTeacherResponseModel> Create(CourseTeacherRequestModel model)
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var course = await courseService
                .GetByIdOrSlugAsync(model.CourseIdentity, CurrentUser.Id)
                .ConfigureAwait(false);
            if (course.Status == CourseStatus.Completed)
            {
                throw new InvalidOperationException(localizer.GetString("CompletedCourseIssue"));
            }

            var user =
                await userService.GetUserByEmailAsync(model.Email).ConfigureAwait(false)
                ?? throw new EntityNotFoundException(localizer.GetString("UserNotFound"));
            if (user.Role == UserRole.Trainee)
            {
                throw new InvalidOperationException(localizer.GetString("TraineeCannotBeTrainer"));
            }

            var currentTimeStamp = DateTime.UtcNow;
            var courseTeacher = new CourseTeacher
            {
                CourseId = course.Id,
                UserId = user.Id,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
            };
            var response = await courseTeacherService
                .CreateAsync(courseTeacher)
                .ConfigureAwait(false);
            return new CourseTeacherResponseModel(response);
        }

        /// <summary>
        /// Deletes the course teacher.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await courseTeacherService
                .DeleteAsync(id.ToString(), CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel
                {
                    Success = true,
                    Message = localizer.GetString("TrainingTrainer")
                }
            );
        }
    }
}
