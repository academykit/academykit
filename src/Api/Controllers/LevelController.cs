// <copyright file="LevelController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    public class LevelController : BaseApiController
    {
        private readonly ILevelService levelService;
        private readonly IValidator<LevelRequestModel> validator;

        public LevelController(
            ILevelService levelService,
            IValidator<LevelRequestModel> validator)
        {
            this.levelService = levelService;
            this.validator = validator;
        }

        /// <summary>
        /// get level api.
        /// </summary>
        /// <returns> the list of <see cref="LevelResponseModel" /> .</returns>
        [HttpGet]
        public async Task<IList<LevelResponseModel>> SearchAsync()
        {
            var levels = await levelService.GetLevelsAsync().ConfigureAwait(false);
            return levels.Select(x => new LevelResponseModel(x)).ToList();
        }

        /// <summary>
        /// tag level api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="LevelRequestModel" />. </param>
        /// <returns> the instance of <see cref="LevelResponseModel" /> .</returns>
        [HttpPost]
        public async Task<LevelResponseModel> CreateLevel(LevelRequestModel model)
        {
            await validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var response = await levelService.CreateLevelAsync(model.Name, CurrentUser.Id).ConfigureAwait(false);
            return new LevelResponseModel(response);
        }

        /// <summary>
        /// update level api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="LevelRequestModel" />. </param>
        /// <returns> the instance of <see cref="LevelResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<LevelResponseModel> UpdateLevel(string identity, LevelRequestModel model)
        {
            await validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var response = await levelService.UpdateLevelAsync(identity, model.Name, CurrentUser.Id).ConfigureAwait(false);
            return new LevelResponseModel(response);
        }

        /// <summary>
        /// delete level api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteLevel(string identity)
        {
            await levelService.DeleteLevelAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok();
        }
    }
}
