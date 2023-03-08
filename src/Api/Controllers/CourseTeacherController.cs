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
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class CourseTeacherController : BaseApiController
    {
        private readonly ICourseTeacherService _courseTeacherService;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly IValidator<CourseTeacherRequestModel> _validator;

        public CourseTeacherController(
                ICourseTeacherService courseTeacherService,
                ICourseService courseService,
                IUserService userService,
                IValidator<CourseTeacherRequestModel> validator)
        {
            _courseTeacherService = courseTeacherService;
            _courseService = courseService;
            _userService = userService;
            _validator = validator;
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
            var user = await _userService.GetUserByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                throw new EntityNotFoundException("User not found.");
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
            return Ok(new CommonResponseModel { Success = true, Message = "Training trainer removed successfully." });
        }
    }
}
