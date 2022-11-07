namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
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
        public CourseController(
            ICourseService courseService,
            IValidator<CourseRequestModel> validator)
        {
            _courseService = courseService;
            _validator = validator;
        }

        /// <summary>
        /// course search api
        /// </summary>
        /// <returns> the list of <see cref="DepartmentResponseModel" /> .</returns>
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
        /// <param name="model"> the instance of <see cref="CourseRequestModelv" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPost]
        public async Task<CourseResponseModel> CreateAsync(CourseRequestModel model)
        {
            IsAdmin(CurrentUser.Role);

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
                Status = Status.Draft,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                CourseTags = new List<CourseTag>(),
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
            var response = await _courseService.CreateAsync(entity).ConfigureAwait(false);
            return new CourseResponseModel(response);
        }

        /// <summary>
        /// update department api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="CourseRequestModel" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<CourseResponseModel> UpdateAsync(string identity, CourseRequestModel model)
        {
            IsAdmin(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _courseService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _courseService.UpdateAsync(existing).ConfigureAwait(false);
            return new CourseResponseModel(savedEntity);
        }

        /// <summary>
        /// delete department api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<CommonResponseModel> DeletAsync(string identity)
        {
            IsAdmin(CurrentUser.Role);

            await _courseService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return new CommonResponseModel() { Success = true, Message = "Course removed successfully." };
        }

        
    }
}
