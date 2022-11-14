namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class SectionController : BaseApiController
    {
        private readonly ICourseService _courseService;
        private readonly ISectionService _sectionService;
        private readonly IValidator<SectionRequestModel> _validator;
        public SectionController(
            ICourseService courseService,
            ISectionService sectionService,
            IValidator<SectionRequestModel> validator)
        {
            _courseService = courseService;
            _sectionService = sectionService;
            _validator = validator;
        }

        /// <summary>
        /// section search api
        /// </summary>
        /// <returns> the list of <see cref="SectionResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<SectionResponseModel>> SearchAsync([FromQuery] SectionBaseSearchCriteria searchCriteria)
        {
            var searchResult = await _sectionService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<SectionResponseModel>
            {
                Items = new List<SectionResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new SectionResponseModel(p, fetchLesson: true))
             );
            return response;
        }

        /// <summary>
        /// create section api
        /// </summary>
        /// <param name="model"> the instance of <see cref="SectionRequestModel" /> .</param>
        /// <returns> the instance of <see cref="SectionResponseModel" /> .</returns>
        [HttpPost]
        public async Task<SectionResponseModel> Create(SectionRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var course = await _courseService.GetByIdOrSlugAsync(model.CourseIdentity,currentUserId:CurrentUser.Id).ConfigureAwait(false);
            if (course == null)
            {
                throw new EntityNotFoundException("Course not found");
            }
            var entity = new Section()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                CourseId = course.Id,
                CreatedBy = CurrentUser.Id,
                CreatedOn = DateTime.UtcNow
            };
            var response = await _sectionService.CreateAsync(entity).ConfigureAwait(false);
            return new SectionResponseModel(response);
        }

        /// <summary>
        /// get section by id or slug
        /// </summary>
        /// <param name="identity"> the section id or slug</param>
        /// <returns> the instance of <see cref="SectionResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<SectionResponseModel> Get(string identity)
        {
            var model = await _sectionService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return new SectionResponseModel(model);
        }

        /// <summary>
        /// update section api
        /// </summary>
        /// <param name="model"> the instance of <see cref="SectionRequestModel" /> .</param>
        /// <returns> the instance of <see cref="SectionResponseModel" /> .</returns>
        [HttpPatch("{identity}")]
        public async Task<SectionResponseModel> Update(string identity, SectionRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var course = await _courseService.GetByIdOrSlugAsync(model.CourseIdentity).ConfigureAwait(false);
            if (course == null)
            {
                throw new EntityNotFoundException("Course not found");
            }
            var entity = await _sectionService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);

            if (course == null)
            {
                throw new EntityNotFoundException("Section not found");
            }


            entity.Name = model.Name;
            entity.CourseId = course.Id;
            entity.UpdatedBy = CurrentUser.Id;
            entity.UpdatedOn = DateTime.UtcNow;

            var response = await _sectionService.UpdateAsync(entity).ConfigureAwait(false);
            return new SectionResponseModel(response);
        }


        /// <summary>
        /// delete section api
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> Delete(string identity)
        {
            await _sectionService.DeleteSectionAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok();
        }
    }
}