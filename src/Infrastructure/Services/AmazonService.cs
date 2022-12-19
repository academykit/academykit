namespace Lingtren.Infrastructure.Services
{
    using Amazon.S3;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;

    public class AmazonService : BaseService, IAmazonService
    {

        private readonly AmazonS3Client s3Client;

        public AmazonService(IUnitOfWork unitOfWork,
        ILogger<AmazonService> logger) : base(unitOfWork, logger)
        {
            var setting = GetAwsSetting();
            s3Client = new AmazonS3Client(setting.AccessKey, setting.SecretKey, setting.RegionEndpoint);
        }


        #region  private 

        /// <summary>
        /// Handle to get aws settings
        /// </summary>
        /// <returns> the instance of <see cref="AmazonAccessModel" /> .</returns>
        private AmazonAccessModel GetAwsSetting()
        {
            var awsSetting = new AmazonAccessModel();
            var settings = _unitOfWork.GetRepository<Setting>().GetAll(predicate: x => x.Key.StartsWith("AWS"));
            var accessKey = settings.FirstOrDefault(x => x.Key == "AWS_AccessKey")?.Value;
            if (string.IsNullOrEmpty(accessKey))
            {
                throw new EntityNotFoundException("Aws Access key not found.");
            }

            var secretKey = settings.FirstOrDefault(x => x.Key == "AWS_SecretKey")?.Value;
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new EntityNotFoundException("AWS secret key not found.");
            }

            var regionEndPoint = settings.FirstOrDefault(x => x.Key == "AWS_RegionEndpoint")?.Value;
            if (string.IsNullOrEmpty(regionEndPoint))
            {
                throw new EntityNotFoundException("Aws region end point not found.");
            }
            return new AmazonAccessModel
            {
                AccessKey = accessKey,
                SecretKey = secretKey,
                RegionEndpoint = regionEndPoint
            };
        }

        #endregion
    }
}