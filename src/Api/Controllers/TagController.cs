namespace Lingtren.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class TagController : BaseApiController
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// get tag api
        /// </summary>
        /// <param name="searchCriteria"> the instance of <see cref="BaseSearchCriteria" /> .</param>
        /// <returns> the list of <see cref="TagResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<TagResponseModel>> SearchAsync([FromQuery] BaseSearchCriteria searchCriteria)
        {
            var searchResult = await _tagService.SearchAsync(searchCriteria, includeAllProperties: false).ConfigureAwait(false);

            var response = new SearchResult<TagResponseModel>
            {
                Items = new List<TagResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new TagResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// tag create api
        /// </summary>
        /// <param name="model"> the instance of <see cref="TagRequestModel" />. </param>
        /// <returns> the instance of <see cref="TagResponseModel" /> .</returns>
        [HttpPost]
        public async Task<TagResponseModel> CreateTag(TagRequestModel model)
        {
            var response = await _tagService.CreateTagAsync(model.Name,CurrentUser.Id).ConfigureAwait(false);
            return new TagResponseModel(response);
        }

        /// <summary>
        /// update tag api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="TagRequestModel" />. </param>
        /// <returns> the instance of <see cref="TagResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<TagResponseModel> UpdateTag(string identity, TagRequestModel model)
        {
            var existing = await _tagService.GetByIdOrSlugAsync(identity, CurrentUser.Id.ToString(),false).ConfigureAwait(false);

            existing.Name = model.Name;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = DateTime.UtcNow;

            var savedEntity = await _tagService.UpdateAsync(existing,false).ConfigureAwait(false);
            return new TagResponseModel(savedEntity);
        }

        /// <summary>
        /// delete tag api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteTag(string identity)
        {
            await _tagService.DeleteTagAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return NoContent();
        }
    }
}