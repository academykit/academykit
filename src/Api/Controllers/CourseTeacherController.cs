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
        private readonly ICourseTeacherService _courseTeacherService;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly IValidator<CourseTeacherRequestModel> _validator;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
        public CourseTeacherController(
                ICourseTeacherService courseTeacherService,
                ICourseService courseService,
                IUserService userService,
                IValidator<CourseTeacherRequestModel> validator,
                IStringLocalizer<ExceptionLocalizer> localizer)
        {
            _courseTeacherService = courseTeacherService;
            _courseService = courseService;
            _userService = userService;
            _validator = validator;
            _localizer = localizer;
        }

        /// <summary>
        /// Searches the question pool moderators.
        /// </summary>
        /// <param name="criteria">The search criteria</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<SearchResult<CourseTeacherResponseModel>> Search([FromQuery] CourseTeacherSearchCriteria criteria)
        {
            // question pool id is required
            CommonHelper.ValidateArgumentNotNullOrEmpty(criteria.CourseIdentity, nameof(criteria.CourseIdentity));
            criteria.CurrentUserId = CurrentUser.Id;
            var searchResult = await _courseTeacherService.SearchAsync(criteria).ConfigureAwait(false);

            var response = new SearchResult<CourseTeacherResponseModel>
            {
                Items = new List<CourseTeacherResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                response.Items.Add(new CourseTeacherResponseModel(p))
            );

            return response;
        }

        /// <summary>
        /// add new moderator
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseTeacherRequestModel" /> .</param>
        /// <returns> the instance of <see cref="CourseTeacherResponseModel" /> .</returns>
        [HttpPost()]
        public async Task<CourseTeacherResponseModel> Create(CourseTeacherRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var course = await _courseService.GetByIdOrSlugAsync(model.CourseIdentity, CurrentUser.Id).ConfigureAwait(false);
            if (course.Status == CourseStatus.Completed)
            {
                throw new InvalidOperationException(_localizer.GetString("CompletedCourseIssue"));
            }
            var user = await _userService.GetUserByEmailAsync(model.Email).ConfigureAwait(false) ?? throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
            if (user.Role == UserRole.Trainee)
            {
                throw new InvalidOperationException(_localizer.GetString("TraineeCannotBeTrainer"));
            }

            var currentTimeStamp = DateTime.UtcNow;
            var courseTeacher = new CourseTeacher
            {
                CourseId = course.Id,
                UserId = user.Id,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp
            };
            var response = await _courseTeacherService.CreateAsync(courseTeacher).ConfigureAwait(false);
            return new CourseTeacherResponseModel(response);

        }

        /// <summary>
        /// Deletes the course teacher
        /// </summary>
        /// <param name="id">The id</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _courseTeacherService.DeleteAsync(id.ToString(), CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel { Success = true, Message = _localizer.GetString("TrainingTrainer") });
        }
    }
}
