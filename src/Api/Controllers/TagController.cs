namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
        /// <param name="searchCriteria"> the instance of <see cref=" TagBaseSearchCriteria" /> .</param>
        /// <returns> the list of <see cref="TagResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<TagResponseModel>> SearchAsync([FromQuery] TagBaseSearchCriteria searchCriteria)
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
            var response = await _tagService.CreateTagAsync(model.Name, CurrentUser.Id).ConfigureAwait(false);
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
            var tag = await _tagService.UpdateTagAsync(identity, model.Name, CurrentUser.Id).ConfigureAwait(false);
            return new TagResponseModel(tag);
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