namespace AcademyKit.Infrastructure.Services
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class CertificateService : BaseService, ICertificateService
    {
        public CertificateService(
            IUnitOfWork unitOfWork,
            ILogger<CertificateService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Handle to save external certificate
        /// </summary>
        /// <param name="model"> the instance of <see cref="CertificateRequestModel" /> .</param>
        /// <param name="currentUserId"> the user id </param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        public async Task<CertificateResponseModel> SaveExternalCertificateAsync(
            CertificateRequestModel model,
            Guid currentUserId
        )
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var certificate = new Certificate
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ImageUrl = model.ImageUrl,
                    Institute = model.Institute,
                    Duration = model.Duration,
                    Status = CertificateStatus.Draft,
                    Location = model.Location,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow,
                    OptionalCost = model.OptionalCost,
                };
                ;
                var isAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (isAdmin)
                {
                    certificate.Status = CertificateStatus.Approved;
                }

                if (model.StartDate.AddHours(model.Duration).Date > model.EndDate)
                {
                    throw new ForbiddenException(_localizer.GetString("AddingDurationError"));
                }

                await _unitOfWork
                    .GetRepository<Certificate>()
                    .InsertAsync(certificate)
                    .ConfigureAwait(false);
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
        public async Task<CertificateResponseModel> UpdateExternalCertificateAsync(
            Guid identity,
            CertificateRequestModel model,
            Guid currentUserId
        )
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var certificate = await _unitOfWork
                    .GetRepository<Certificate>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Id == identity)
                    .ConfigureAwait(false);
                if (certificate == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("CertificateNotFound"));
                }

                if (certificate.CreatedBy != currentUserId)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                if (certificate.Status == CertificateStatus.Approved)
                {
                    throw new ArgumentException(_localizer.GetString("CertificateAlreadyApproved"));
                }

                certificate.Name = model.Name;
                certificate.StartDate = model.StartDate;
                certificate.EndDate = model.EndDate;
                certificate.ImageUrl = model.ImageUrl;
                certificate.Location = model.Location;
                certificate.Institute = model.Institute;
                certificate.Duration = model.Duration;
                certificate.Status = CertificateStatus.Draft;
                certificate.UpdatedBy = currentUserId;
                certificate.UpdatedOn = DateTime.UtcNow;
                certificate.OptionalCost = model.OptionalCost;
                _unitOfWork.GetRepository<Certificate>().Update(certificate);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return new CertificateResponseModel(certificate);
            });
        }

        /// <summary>
        /// Handle to delete the external certificate async
        /// </summary>
        /// <param name="identity"> the certificate id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task DeleteExternalCertificateAsync(Guid identity, Guid currentUserId)
        {
            try
            {
                var certificate = await _unitOfWork
                    .GetRepository<Certificate>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Id == identity)
                    .ConfigureAwait(false);
                if (certificate == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("CertificateNotFound"));
                }

                if (certificate.CreatedBy != currentUserId)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                _unitOfWork.GetRepository<Certificate>().Delete(certificate);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get external certificate
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        public async Task<IList<CertificateResponseModel>> GetExternalCertificateAsync(
            Guid currentUserId
        )
        {
            try
            {
                _logger.LogWarning(currentUserId.ToString());
                var certificates = await _unitOfWork
                    .GetRepository<Certificate>()
                    .GetAllAsync(
                        predicate: p => p.CreatedBy == currentUserId,
                        include: source => source.Include(x => x.User)
                    )
                    .ConfigureAwait(false);
                _logger.LogError(certificates.Count.ToString());

                var response = certificates
                    .Select(x => new CertificateResponseModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Institute = x.Institute,
                        ImageUrl = x.ImageUrl,
                        Duration = x.Duration != default ? x.Duration.ToString() : null,
                        Location = x.Location,
                        Status = x.Status,
                        User = new UserModel(x.User),
                        OptionalCost = x.OptionalCost,
                    })
                    .ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get certificate details
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="CertificateResponseModel" /> .</returns>
        public async Task<CertificateResponseModel> GetCertificateDetailAsync(
            Guid identity,
            Guid currentUserId
        )
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var certificate = await _unitOfWork
                    .GetRepository<Certificate>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == identity,
                        include: source => source.Include(x => x.User)
                    )
                    .ConfigureAwait(false);

                if (certificate == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("CertificateNotFound"));
                }

                if (
                    certificate.Status == CertificateStatus.Draft
                    || certificate.Status == CertificateStatus.Rejected
                )
                {
                    var isAccess = await UnverifiedCertificateAccess(certificate, currentUserId)
                        .ConfigureAwait(false);
                    if (!isAccess)
                    {
                        throw new ArgumentException(_localizer.GetString("CertificateNotVerified"));
                    }
                }

                return new CertificateResponseModel
                {
                    Id = certificate.Id,
                    Name = certificate.Name,
                    StartDate = certificate.StartDate,
                    EndDate = certificate.EndDate,
                    Institute = certificate.Institute,
                    ImageUrl = certificate.ImageUrl,
                    Duration =
                        certificate.Duration != default ? certificate.Duration.ToString() : null,
                    Location = certificate.Location,
                    Status = certificate.Status,
                    User = new UserModel(certificate.User),
                    OptionalCost = certificate.OptionalCost
                };
            });
        }

        /// <summary>
        /// Handle to get user certificates
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> </returns>
        public async Task<IList<CertificateResponseModel>> GetUserCertificateAsync(Guid userId)
        {
            try
            {
                var certificates = await _unitOfWork
                    .GetRepository<Certificate>()
                    .GetAllAsync(
                        predicate: p =>
                            p.CreatedBy == userId && p.Status == CertificateStatus.Approved,
                        include: source => source.Include(x => x.User)
                    )
                    .ConfigureAwait(false);

                var response = certificates
                    .Select(x => new CertificateResponseModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Institute = x.Institute,
                        ImageUrl = x.ImageUrl,
                        Duration = x.Duration != default ? x.Duration.ToString() : null,
                        Location = x.Location,
                        Status = x.Status,
                        User = new UserModel(x.User),
                        OptionalCost = x.OptionalCost
                    })
                    .ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get unverified certificate
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="CertificateBaseSearchCriteria" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="CertificateResponseModel" /> .</returns>
        public async Task<SearchResult<CertificateResponseModel>> GetReviewCertificatesAsync(
            CertificateBaseSearchCriteria criteria,
            Guid currentUserId
        )
        {
            try
            {
                var hasAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var certificates = await _unitOfWork
                    .GetRepository<Certificate>()
                    .GetAllAsync(
                        predicate: p => p.Status == CertificateStatus.Draft,
                        include: source => source.Include(x => x.User)
                    )
                    .ConfigureAwait(false);

                var response = certificates
                    .Select(x => new CertificateResponseModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Institute = x.Institute,
                        ImageUrl = x.ImageUrl,
                        Duration = x.Duration != default ? x.Duration.ToString() : null,
                        Location = x.Location,
                        Status = x.Status,
                        User = new UserModel(x.User),
                        OptionalCost = x.OptionalCost,
                    })
                    .ToList();
                return response.ToIPagedList(criteria.Page, criteria.Size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to verify certificate
        /// </summary>
        /// <param name="identity"> the certificate id or slug </param>
        /// <param name="status"> the certificate status </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task VerifyCertificateAsync(
            Guid identity,
            CertificateStatus status,
            Guid currentUserId
        )
        {
            try
            {
                var hasAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var certificate = await _unitOfWork
                    .GetRepository<Certificate>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Id == identity)
                    .ConfigureAwait(false);

                if (certificate == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("CertificateNotFound"));
                }

                certificate.Status = status;
                _unitOfWork.GetRepository<Certificate>().Update(certificate);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get internal certificate
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <returns> the list of <see cref="CourseCertificateIssuedResponseModel" /> .</returns>
        public async Task<IList<CourseCertificateIssuedResponseModel>> GetInternalCertificateAsync(
            Guid userId
        )
        {
            try
            {
                var userCertificates = await _unitOfWork
                    .GetRepository<CourseEnrollment>()
                    .GetAllAsync(
                        predicate: p =>
                            p.UserId == userId
                            && p.HasCertificateIssued.HasValue
                            && p.HasCertificateIssued.Value,
                        include: src => src.Include(x => x.Course)
                    )
                    .ConfigureAwait(false);

                var response = userCertificates
                    .Select(item => new CourseCertificateIssuedResponseModel
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.Name,
                        CourseSlug = item.Course.Slug,
                        Percentage = item.Percentage,
                        HasCertificateIssued = item.HasCertificateIssued,
                        CertificateIssuedDate = item.CertificateIssuedDate,
                        CertificateUrl = item.CertificateUrl,
                    })
                    .ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
                throw;
            }
        }

        #region  private method

        /// <summary>
        /// Handle to check unverified certificate access
        /// </summary>
        /// <param name="certificate"> the instance of <see cref="Certificate" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the boolean value </returns>
        private async Task<bool> UnverifiedCertificateAccess(
            Certificate certificate,
            Guid currentUserId
        )
        {
            if (certificate.CreatedBy == currentUserId)
            {
                return true;
            }

            return await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
        }

        #endregion
    }
}
