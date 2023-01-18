namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Enums;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/certificate")]
    public class ExternalCertificateController : BaseApiController
    {
        private readonly ICertificateService _certificateService;
        public ExternalCertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        /// <summary>
        /// add external certificate api
        /// </summary>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        [HttpPost("external")]
        public async Task<CertificateResponseModel> External(CertificateRequestModel model)
        {
            var response = await _certificateService.SaveExternalCertificateAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// update external certificate api
        /// </summary>
        /// <param name="identity"> the ceritificate id or slug </param>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        [HttpPut("{identity}/external")]
        public async Task<CertificateResponseModel> UpdateExternal(Guid identity,CertificateRequestModel model)
        {
            var response = await _certificateService.UpdateExternalCertificateAsync(identity,model,CurrentUser.Id).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// delete external certificate api
        /// </summary>
        /// <param name="identity"> the certificate id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}/external")]
        public async Task<IActionResult> DeleteExternal(Guid identity)
        {
            await _certificateService.DeleteExternalCertificateAsync(identity,CurrentUser.Id).ConfigureAwait(false);
             return Ok();
        }

        /// <summary>
        /// get external certificate api
        /// </summary>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        [HttpGet("external")]
        public async Task<IList<CertificateResponseModel>> GetExternal() => await _certificateService.GetExternalCertificateAsync(CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get internal certificate api
        /// </summary>
        /// <returns> the list of <see cref="CourseCertificateIssuedResponseModel" /> .</returns>
        [HttpGet("internal")]
        public async Task<IList<CourseCertificateIssuedResponseModel>> GetInternal() => await _certificateService.GetInternalCertificateAsync(CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get certificate api
        /// </summary>
        /// <param name="identity"> the certificate id or slug </param>
        /// <returns> the instance of <see cerf="CertificateResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<CertificateResponseModel> Certificate(Guid identity) => await _certificateService.GetCertificateDetailAsync(identity,CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// verify certificate api
        /// </summary>
        /// <param name="identity"> the certificate id or slug </param>
        /// <returns> the task complete </returns>
        [HttpPatch("{identity}/verify")]
        public async Task<IActionResult> Verify(Guid identity, [FromQuery] CertificateStatus status)
        {
            await _certificateService.VerifyCertificateAsync(identity,status,CurrentUser.Id).ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// get user certificate
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        [HttpGet("external/{userId}")]
        public async Task<IList<CertificateResponseModel>> UserCertificate(Guid userId) => await _certificateService.GetUserCertificateAsync(userId).ConfigureAwait(false);

        /// <summary>
        /// get in review certificate async
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="CertificateBaseSearchCriteria" /> .</param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        [HttpGet("review")]
        public async Task<SearchResult<CertificateResponseModel>> GetReviews([FromQuery]CertificateBaseSearchCriteria criteria)
        {
            var searchResult = await _certificateService.GetReviewCertificatesAsync(criteria,CurrentUser.Id).ConfigureAwait(false);
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