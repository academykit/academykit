namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/course/{identity}/certificate")]
    public class CertificateController : BaseApiController
    {
        private readonly ICourseService courseService;
        private readonly IValidator<CourseCertificateRequestModel> validator;

        public CertificateController(
            ICourseService courseService,
            IValidator<CourseCertificateRequestModel> validator
        )
        {
            this.courseService = courseService;
            this.validator = validator;
        }

        /// <summary>
        /// course certificate search api.
        /// </summary>
        /// <returns> the list of <see cref="CourseCertificateResponseModel" /> .</returns>
        [HttpGet("detail")]
        public async Task<CourseCertificateResponseModel> GetDetail(string identity)
        {
            return await courseService
                .GetCertificateDetailAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// course  certificate issue api.
        /// </summary>
        /// <param name="identity">the course id or slug.</param>
        /// <param name="model">the instance of <see cref="CertificateIssueRequestModel"/>.</param>
        /// <returns>the list of <see cref="CourseCertificateResponseModel"/>.</returns>
        [HttpPost]
        public async Task<CourseCertificateResponseModel> CertificateDetail(
            string identity,
            CourseCertificateRequestModel model
        )
        {
            await validator.ValidateAsync(model, x => x.ThrowOnFailures()).ConfigureAwait(false);
            return await courseService
                .InsertCertificateDetail(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// course  certificate issue api.
        /// </summary>
        /// <param name="identity">the course id or slug.</param>
        /// <param name="model">the instance of <see cref="CertificateIssueRequestModel"/>.</param>
        /// <returns>the list of <see cref="CourseCertificateResponseModel"/>.</returns>
        [HttpPost("issue")]
        public async Task<IList<CourseCertificateIssuedResponseModel>> IssueCertificateAsync(
            string identity,
            CertificateIssueRequestModel model
        )
        {
            return await courseService
                .IssueCertificateAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
        }
    }
}
