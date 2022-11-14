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
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class CourseController : BaseApiController
    {
        private readonly ICourseService _courseService;
        private readonly IValidator<CourseRequestModel> _validator;
        private readonly ILogger<CourseController> _logger;

        public CourseController(
            ICourseService courseService,
            IValidator<CourseRequestModel> validator,
            ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// course search api
        /// </summary>
        /// <returns> the list of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<CourseResponseModel>> SearchAsync([FromQuery] CourseBaseSearchCriteria searchCriteria)
        {
            var searchResult = await _courseService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<CourseResponseModel>
            {
                Items = new List<CourseResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new CourseResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// create course api
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseRequestModel" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPost]
        public async Task<CourseResponseModel> CreateAsync(CourseRequestModel model)
        {
            IsTeacherAdmin(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new Course
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Language = model.Language,
                GroupId = model.GroupId,
                LevelId = model.LevelId,
                Duration = model.Duration,
                Description = model.Description,
                ThumbnailUrl = model.ThumbnailUrl,
                Status = CourseStatus.Draft,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                CourseTags = new List<CourseTag>(),
                CourseTeachers = new List<CourseTeacher>()
            };
            foreach (var tagId in model.TagIds)
            {
                entity.CourseTags.Add(new CourseTag
                {
                    Id = Guid.NewGuid(),
                    TagId = tagId,
                    CourseId = entity.Id,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = CurrentUser.Id,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = CurrentUser.Id,
                });
            }
            entity.CourseTeachers.Add(new CourseTeacher
            {
                Id = Guid.NewGuid(),
                CourseId = entity.Id,
                UserId = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
            });

            var response = await _courseService.CreateAsync(entity).ConfigureAwait(false);
            return new CourseResponseModel(response);
        }

        /// <summary>
        /// get course by id or slug
        /// </summary>
        /// <param name="identity"> the course id or slug</param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<CourseResponseModel> Get(string identity)
        {
            var model = await _courseService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return new CourseResponseModel(model);
        }

        /// <summary>
        /// update course api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="CourseRequestModel" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<CourseResponseModel> UpdateAsync(string identity, CourseRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _courseService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.Language = model.Language;
            existing.GroupId = model.GroupId;
            existing.LevelId = model.LevelId;
            existing.Duration = model.Duration;
            existing.Description = model.Description;
            existing.ThumbnailUrl = model.ThumbnailUrl;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;
            existing.CourseTags = new List<CourseTag>();

            foreach (var tagId in model.TagIds)
            {
                existing.CourseTags.Add(new CourseTag
                {
                    Id = Guid.NewGuid(),
                    TagId = tagId,
                    CourseId = existing.Id,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = CurrentUser.Id,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = CurrentUser.Id,
                });
            }

            var savedEntity = await _courseService.UpdateAsync(existing).ConfigureAwait(false);
            return new CourseResponseModel(savedEntity);
        }

        /// <summary>
        /// delete course api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeletAsync(string identity)
        {
            await _courseService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Course removed successfully." });
        }

        /// <summary>
        /// change course status api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpPatch("{identity}/status")]
        public async Task<IActionResult> ChangeStatus(string identity, [FromQuery] CourseStatus status)
        {
            var statusExists = Enum.IsDefined(typeof(CourseStatus), status);
            if (!statusExists)
            {
                _logger.LogWarning("Invalid course status : {status} requested for status change by the user with id : {userId}", status, CurrentUser.Id);
                throw new ForbiddenException("Invalid course status requested");
            }
            await _courseService.ChangeStatusAsync(identity, status, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Course status changed successfully." });
        }
    }
}
