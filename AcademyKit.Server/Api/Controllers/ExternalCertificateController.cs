namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Helpers;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/certificate")]
    public class ExternalCertificateController : BaseApiController
    {
        private readonly ICertificateService certificateService;
        private readonly IValidator<CertificateRequestModel> validator;

        public ExternalCertificateController(
            ICertificateService certificateService,
            IValidator<CertificateRequestModel> validator
        )
        {
            this.certificateService = certificateService;
            this.validator = validator;
        }

        /// <summary>
        /// add external certificate api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        [HttpPost("external")]
        public async Task<CertificateResponseModel> External(CertificateRequestModel model)
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var response = await certificateService
                .SaveExternalCertificateAsync(model, CurrentUser.Id)
                .ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// update external certificate api.
        /// </summary>
        /// <param name="identity"> the ceritificate id or slug. </param>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        [HttpPut("{identity}/external")]
        public async Task<CertificateResponseModel> UpdateExternal(
            Guid identity,
            CertificateRequestModel model
        )
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(model.Name, nameof(model.Name));
            var response = await certificateService
                .UpdateExternalCertificateAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// delete external certificate api.
        /// </summary>
        /// <param name="identity"> the certificate id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}/external")]
        public async Task<IActionResult> DeleteExternal(Guid identity)
        {
            await certificateService
                .DeleteExternalCertificateAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// get external certificate api.
        /// </summary>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        [HttpGet("external")]
        public async Task<IList<CertificateResponseModel>> GetExternal() =>
            await certificateService
                .GetExternalCertificateAsync(CurrentUser.Id)
                .ConfigureAwait(false);

        /// <summary>
        /// get internal certificate api.
        /// </summary>
        /// <returns> the list of <see cref="CourseCertificateIssuedResponseModel" /> .</returns>
        [HttpGet("internal")]
        public async Task<IList<CourseCertificateIssuedResponseModel>> GetInternal() =>
            await certificateService
                .GetInternalCertificateAsync(CurrentUser.Id)
                .ConfigureAwait(false);

        /// <summary>
        /// get certificate api.
        /// </summary>
        /// <param name="identity"> the certificate id or slug. </param>
        /// <returns> the instance of <see cerf="CertificateResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<CertificateResponseModel> Certificate(Guid identity) =>
            await certificateService
                .GetCertificateDetailAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);

        /// <summary>
        /// verify certificate api.
        /// </summary>
        /// <param name="identity"> the certificate id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpPatch("{identity}/verify")]
        public async Task<IActionResult> Verify(Guid identity, [FromQuery] CertificateStatus status)
        {
            await certificateService
                .VerifyCertificateAsync(identity, status, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// get user certificate.
        /// </summary>
        /// <param name="userId"> the user id. </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        [HttpGet("external/{userId}")]
        public async Task<IList<CertificateResponseModel>> UserCertificate(Guid userId) =>
            await certificateService.GetUserCertificateAsync(userId).ConfigureAwait(false);

        /// <summary>
        /// get in review certificate async.
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="CertificateBaseSearchCriteria" /> .</param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        [HttpGet("review")]
        public async Task<SearchResult<CertificateResponseModel>> GetReviews(
            [FromQuery] CertificateBaseSearchCriteria criteria
        )
        {
            var searchResult = await certificateService
                .GetReviewCertificatesAsync(criteria, CurrentUser.Id)
                .ConfigureAwait(false);
            var response = new SearchResult<CertificateResponseModel>()
            {
                Items = searchResult.Items,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };
            return response;
        }
    }
}
