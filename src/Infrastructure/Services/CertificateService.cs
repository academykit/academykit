namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Microsoft.Extensions.Logging;

    public class CertificateService : BaseService, ICertificateService
    {
        public CertificateService(IUnitOfWork unitOfWork, ILogger<CertificateService> logger) : base(unitOfWork, logger)
        {
        }

        /// <summary>
        /// Handle to save external certificate 
        /// </summary>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <param name="currentUserId"> the user id </param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        public async Task<CertificateResponseModel> SaveExternalCertificateAsync(CertificateRequestModel model, Guid currentUserId)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var slug = CommonHelper.GetEntityTitleSlug<Certificate>(_unitOfWork, (slug) => q => q.Slug == slug, model.Name);
                var certificate = new Certificate
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Slug = slug,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ImageUrl = model.ImageUrl,
                    Institute = model.Institute,
                    Duration = model.Duration,
                    IsVerified = false,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Certificate>().InsertAsync(certificate).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return new CertificateResponseModel(certificate);
            });
        }

        /// <summary>
        /// Handle to update the external certificate 
        /// </summary>
        /// <param name="identity"> certificate id or slug </param>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns></returns>
        public async Task<CertificateResponseModel> UpdateExternalCertificateAsync(string identity, CertificateRequestModel model, Guid currentUserId)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var ceritificate = await _unitOfWork.GetRepository<Certificate>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == identity ||
                p.Slug.Equals(identity)).ConfigureAwait(false);
                if (ceritificate == null)
                {
                    throw new EntityNotFoundException($"Certificate with identity : {identity} not found.");
                }

                if (ceritificate.CreatedBy != currentUserId)
                {
                    throw new ForbiddenException("Unauthorized user.");
                }

                if (ceritificate.IsVerified)
                {
                    throw new ArgumentException("Cerificate with identity : {identity} is already verified.");
                }

                ceritificate.Name = model.Name;
                ceritificate.StartDate = model.StartDate;
                ceritificate.EndDate = model.EndDate;
                ceritificate.ImageUrl = model.ImageUrl;
                ceritificate.Institute = model.Institute;
                ceritificate.Duration = model.Duration;
                ceritificate.UpdatedBy = currentUserId;
                ceritificate.UpdatedOn = DateTime.UtcNow;
                return new CertificateResponseModel(ceritificate);
            });
        }

         /// <summary>
        /// Handle to delete the external certificate async
        /// </summary>
        /// <param name="identity"> the ceritifcate id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task DeleteExternalCertificateAsync(string identity,Guid currentUserId)
        {
            try
            {
                 var ceritificate = await _unitOfWork.GetRepository<Certificate>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == identity ||
                p.Slug.Equals(identity)).ConfigureAwait(false);
                if (ceritificate == null)
                {
                    throw new EntityNotFoundException($"Certificate with identity : {identity} not found.");
                }

                if (ceritificate.CreatedBy != currentUserId)
                {
                    throw new ForbiddenException("Unauthorized user.");
                }
                _unitOfWork.GetRepository<Certificate>().Delete(ceritificate);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }
    }
}