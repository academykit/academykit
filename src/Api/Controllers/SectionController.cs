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

    [Route("api/course/{identity}/section")]
    public class SectionController : BaseApiController
    {
        private readonly ICourseService _courseService;
        private readonly ISectionService _sectionService;
        private readonly IValidator<SectionRequestModel> _validator;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
        public SectionController(
            ICourseService courseService,
            ISectionService sectionService,
            IValidator<SectionRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer)
        {
            _courseService = courseService;
            _sectionService = sectionService;
            _validator = validator;
            _localizer = localizer;
        }

        /// <summary>
        /// section search api
        /// </summary>
        /// <returns> the list of <see cref="SectionResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<SectionResponseModel>> SearchAsync(string identity, [FromQuery] SectionBaseSearchCriteria searchCriteria)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(identity, nameof(identity));
            var course = await _courseService.GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id).ConfigureAwait(false) ?? throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            searchCriteria.CurrentUserId = CurrentUser.Id;
            searchCriteria.CourseId = course.Id;

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
        public async Task<SectionResponseModel> Create(string identity, SectionRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ForbiddenException(_localizer.GetString("SectionNameCannotBeNull"));
            }
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var course = await _courseService.GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id).ConfigureAwait(false) ?? throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            if (course.Status == CourseStatus.Completed)
            {
                throw new ArgumentException(_localizer.GetString("CourseCompleted"));
            }

            var entity = new Section()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Status = CourseStatus.Draft,
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
        [HttpGet("{sectionIdentity}")]
        public async Task<SectionResponseModel> Get(string identity, string sectionIdentity)
        {
            var course = await _courseService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false) ?? throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            var model = await _sectionService.GetByIdOrSlugAsync(sectionIdentity, CurrentUser.Id).ConfigureAwait(false);
            return new SectionResponseModel(model, fetchLesson: true);
        }

        /// <summary>
        /// update section api
        /// </summary>
        /// <param name="model"> the instance of <see cref="SectionRequestModel" /> .</param>
        /// <returns> the instance of <see cref="SectionResponseModel" /> .</returns>
        [HttpPatch("{sectionIdentity}")]
        public async Task<SectionResponseModel> Update(string identity, string sectionIdentity, SectionRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);

            var course = await _courseService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false) ?? throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            var entity = await _sectionService.GetByIdOrSlugAsync(sectionIdentity, CurrentUser.Id).ConfigureAwait(false) ?? throw new EntityNotFoundException(_localizer.GetString("SectionNotFound"));
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
        [HttpDelete("{sectionIdentity}")]
        public async Task<IActionResult> Delete(string identity, string sectionIdentity)
        {
            await _sectionService.DeleteSectionAsync(identity, sectionIdentity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("SectionRemoved") });
        }

        /// <summary>
        /// section reorder api
        /// </summary>
        /// <param name="identity"> the course id or slug</param>
        /// <param name="Ids"> ids of section.</param>
        /// <returns> the task complete</returns>
        [HttpPut("reorder")]
        public async Task<IActionResult> SectionOrder(string identity, IList<Guid> Ids)
        {
            await _sectionService.ReorderAsync(identity, Ids, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("SectionReorder") });
        }
    }
}