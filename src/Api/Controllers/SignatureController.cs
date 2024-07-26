namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Infrastructure.Localization;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/course/{identity}/signature")]
    public class SignatureController : BaseApiController
    {
        private readonly ICourseService courseService;
        private readonly IValidator<SignatureRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public SignatureController(
            ICourseService courseService,
            IValidator<SignatureRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.courseService = courseService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// get signature list api.
        /// </summary>
        /// <returns> the list of <see cref="SignatureResponseModel" /> .</returns>
        [HttpGet]
        public async Task<IList<SignatureResponseModel>> SearchAsync(string identity)
        {
            return await courseService
                .GetAllSignatureAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// create signature api.
        /// </summary>
        /// <param name="identity">the course id or slug. </param>
        /// <param name="model"> the instance of <see cref="SignatureRequestModel" />. </param>
        /// <returns> the instance of <see cref="SignatureResponseModel" /> .</returns>
        [HttpPost]
        public async Task<SignatureResponseModel> CreateAsync(
            string identity,
            SignatureRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await courseService
                .InsertSignatureAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// update signature api.
        /// </summary>
        /// <param name="identity">the course id or slug. </param>
        /// <param name="id">the signature id. </param>
        /// <param name="model"> the instance of <see cref="SignatureRequestModel" />. </param>
        /// <returns> the instance of <see cref="SignatureResponseModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<SignatureResponseModel> UpdateAsync(
            string identity,
            Guid id,
            SignatureRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await courseService
                .UpdateSignatureAsync(identity, id, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// delete signature api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string identity, Guid id)
        {
            await courseService
                .DeleteSignatureAsync(identity, id, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("SignatureRemoved")
                }
            );
        }
    }
}
