namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    public interface ICertificateService
    {
        /// <summary>
        /// Handle to save external certificate 
        /// </summary>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <param name="currentUserId"> the user id </param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        Task<CertificateResponseModel> SaveExternalCertificateAsync(CertificateRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to update the external certificate 
        /// </summary>
        /// <param name="identity"> certificate id or slug </param>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns></returns>
        Task<CertificateResponseModel> UpdateExternalCertificateAsync(string identity, CertificateRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to delete the external certificate async
        /// </summary>
        /// <param name="identity"> the ceritifcate id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task DeleteExternalCertificateAsync(string identity,Guid currentUserId);
    }
}