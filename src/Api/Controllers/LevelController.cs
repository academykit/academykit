namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class LevelController : BaseApiController
    {
        private readonly ILevelService _levelService;
        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }


        /// <summary>
        /// get level api
        /// </summary>
        /// <returns> the list of <see cref="LevelResponseModel" /> .</returns>
        [HttpGet]
        public async Task<IList<LevelResponseModel>> SearchAsync()
        {
            var levels = await _levelService.GetLevelsAsync().ConfigureAwait(false);
            return levels.Select(x => new LevelResponseModel(x)).ToList();
        }

        /// <summary>
        /// tag level api
        /// </summary>
        /// <param name="model"> the instance of <see cref="LevelRequestModel" />. </param>
        /// <returns> the instance of <see cref="LevelResponseModel" /> .</returns>
        [HttpPost]
        public async Task<LevelResponseModel> CreateTag(LevelRequestModel model)
        {
            var response = await _levelService.CreateLevelAsync(model.Name, CurrentUser.Id).ConfigureAwait(false);
            return new LevelResponseModel(response);
        }

        /// <summary>
        /// update level api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="LevelRequestModel" />. </param>
        /// <returns> the instance of <see cref="LevelResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<LevelResponseModel> UpdateTag(string identity, LevelRequestModel model)
        {
            var existing = await _levelService.GetByIdOrSlugAsync(identity, CurrentUser.Id.ToString(), false).ConfigureAwait(false);

            existing.Name = model.Name;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = DateTime.UtcNow;

            var savedEntity = await _levelService.UpdateAsync(existing, false).ConfigureAwait(false);
            return new LevelResponseModel(savedEntity);
        }

        /// <summary>
        /// delete level api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteTag(string identity)
        {
            await _levelService.DeleteLevelAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok();
        }
    }
}