﻿namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/course/{identity}/certificate")]
    public class CertificateController : BaseApiController
    {
        private readonly ICourseService _courseService;
        public CertificateController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// course certificate search api
        /// </summary>
        /// <returns> the list of <see cref="CourseCertificateResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<CourseCertificateResponseModel>> SearchAsync(string identity, [FromQuery] CertificateBaseSearchCriteria searchCriteria)
        {
            return await _courseService.SearchCertificateAsync(identity, searchCriteria, CurrentUser.Id).ConfigureAwait(false);
        }

        [HttpPost]
        public async Task<IList<CourseCertificateResponseModel>> IssueCertificateAsync(string identity, CertificateIssueRequestModel model)
        {
            return await _courseService.IssueCertificateAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
        }

    }
}
