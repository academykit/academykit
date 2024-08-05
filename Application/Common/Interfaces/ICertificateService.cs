namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Enums;

    public interface ICertificateService
    {
        /// <summary>
        /// Handle to save external certificate
        /// </summary>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <param name="currentUserId"> the user id </param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        Task<CertificateResponseModel> SaveExternalCertificateAsync(
            CertificateRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to update the external certificate
        /// </summary>
        /// <param name="identity"> certificate id or slug </param>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns></returns>
        Task<CertificateResponseModel> UpdateExternalCertificateAsync(
            Guid identity,
            CertificateRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to delete the external certificate async
        /// </summary>
        /// <param name="identity"> the ceritifcate id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task DeleteExternalCertificateAsync(Guid identity, Guid currentUserId);

        /// <summary>
        /// Handle to get external certificate
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        Task<IList<CertificateResponseModel>> GetExternalCertificateAsync(Guid currentUserId);

        /// <summary>
        /// Handle to get certificate details
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        Task<CertificateResponseModel> GetCertificateDetailAsync(Guid identity, Guid currentUserId);

        /// <summary>
        /// Handle to verify certificate
        /// </summary>
        /// <param name="identity"> the certificate id or slug </param>
        /// <param name="status"> the certificate status </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task VerifyCertificateAsync(Guid identity, CertificateStatus status, Guid currentUserId);

        /// <summary>
        /// Handle to get user certificates
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> </returns>
        Task<IList<CertificateResponseModel>> GetUserCertificateAsync(Guid userId);

        /// <summary>
        /// Handle to get unverified certificate
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="CertificateBaseSearchCriteria" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        Task<SearchResult<CertificateResponseModel>> GetReviewCertificatesAsync(
            CertificateBaseSearchCriteria criteria,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to get internal certificate
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <returns> the list of <see cref="CourseCertificateIssuedResponseModel" /> .</returns>
        Task<IList<CourseCertificateIssuedResponseModel>> GetInternalCertificateAsync(Guid userId);
    }
}
