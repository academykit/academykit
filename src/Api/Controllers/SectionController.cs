namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Microsoft.AspNetCore.Mvc;

    public class SectionController : BaseApiController
    {
        private readonly ISectionService _sectionService;
        private readonly IValidator<SectionRequestModel> _validator;
        public SectionController(ISectionService sectionService,
        IValidator<SectionRequestModel> validator)
        {
            _sectionService = sectionService;
            _validator = validator;
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

            var entity = new Section()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                CourseId = model.CourseId,
                CreatedBy = CurrentUser.Id,
                CreatedOn = DateTime.UtcNow
            };
            var response = await _sectionService.CreateAsync(entity).ConfigureAwait(false);
            return new SectionResponseModel(response);
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
            var entity = await _sectionService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);

            entity.Name = model.Name;
            entity.CourseId = model.CourseId;
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
            await _sectionService.DeleteSectionAsync(identity,CurrentUser.Id).ConfigureAwait(false);
            return Ok();
        }
    }
}