namespace Lingtren.Infrastructure.Services
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public class AmazonS3Service : BaseService, IAmazonS3Service
    {
        public AmazonS3Service(IUnitOfWork unitOfWork, ILogger<AmazonS3Service>
        logger, IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {

        }

        /// <summary>
        /// Handle to upload file to s3 bucket
        /// </summary>
        /// <param name="file"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the instance of <see cref="MediaFileDto" /> . </returns>
        public async Task<string> UploadFileS3BucketAsync(MediaRequestModel model)
        {
            try
            {
                // need to work on region end point
                var credentails = await GetCredentialAsync().ConfigureAwait(false);
                var client = new AmazonS3Client(credentails.AccessKey, credentails.SecretKey, Amazon.RegionEndpoint.APSouth1);
                var fileName = string.Concat(model.File.FileName.Where(c => !char.IsWhiteSpace(c)));
                var extension = Path.GetExtension(fileName);
                fileName = $"{Guid.NewGuid()}_{fileName}";
                if (string.IsNullOrWhiteSpace(extension))
                {
                    MimeTypes.TryGetExtension(model.File.ContentType, out extension);
                    fileName = $"{Guid.NewGuid()}{extension}";
                }
                var transferUtility = new TransferUtility(client);
                var request = new TransferUtilityUploadRequest
                {
                    Key = fileName,
                    ContentType = model.File.ContentType,
                    InputStream = model.File.OpenReadStream(),
                    BucketName = model.Type == MediaType.Private ? credentails.FileBucket : credentails.VideoBucket
                };
                await transferUtility.UploadAsync(request);
                return model.Type == MediaType.Private ? fileName : $"{credentails.CloudFront}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to save file in s3 bucket.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("S3BucketSave"));
            }
        }

        /// <summary>
        /// Handle to get s3 presigned file url
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the pre-signed file url </returns>
        public async Task<string> GetS3PresignedFileAsync(string key)
        {
            try
            {
                var credentails = await GetCredentialAsync().ConfigureAwait(false);
                var client = new AmazonS3Client(credentails.AccessKey, credentails.SecretKey, Amazon.RegionEndpoint.APSouth1);
                var request = new GetPreSignedUrlRequest
                {
                    Key = key,
                    Expires = DateTime.Now.AddMinutes(60)
                };
                var url = client.GetPreSignedURL(request);
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to get s3 pre-signed file url.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("S3PreSigned"));
            }
        }

        /// <summary>
        /// Handle to save recording file to s3 bucket
        /// </summary>
        /// <param name="dto"> the instance of <see cref="AwsS3FileDto" /> .</param>
        /// <returns> the video url </returns>
        public async Task<string> SaveRecordingFileS3BucketAsync(AwsS3FileDto dto)
        {
            try
            {
                var credentails = await GetCredentialAsync().ConfigureAwait(false);
                var client = new AmazonS3Client(credentails.AccessKey, credentails.SecretKey, Amazon.RegionEndpoint.APSouth1);
                var transferUtility = new TransferUtility(client);
                var request = new TransferUtilityUploadRequest
                {
                    Key = dto.Key,
                    FilePath = dto.FilePath,
                    BucketName = credentails.VideoBucket
                };
                await transferUtility.UploadAsync(request);
                return $"{credentails.CloudFront}/{dto.Key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to save recording file in s3 bucket.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("RecordingFileinS3BucketSave"));
            }
        }

        #region  private

        /// <summary>
        /// Handle to get credential
        /// </summary>
        /// <returns> the instance of <see cref="AmazonSettingDto" /> .</returns>
        private async Task<AmazonSettingDto> GetCredentialAsync()
        {
            try
            {
                var settings = await _unitOfWork.GetRepository<Setting>().GetAllAsync(predicate: x => x.Key.StartsWith("AWS")).ConfigureAwait(false);
                var accessKey = settings.FirstOrDefault(x => x.Key == "AWS_AccessKey")?.Value;
                if (string.IsNullOrEmpty(accessKey))
                {
                    throw new EntityNotFoundException(_localizer.GetString("AwsAccessKeyNotFound"));
                }

                var secretKey = settings.FirstOrDefault(x => x.Key == "AWS_SecretKey")?.Value;
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new EntityNotFoundException(_localizer.GetString("AwsSecretKeyNotFound"));
                }

                var regionEndPoint = settings.FirstOrDefault(x => x.Key == "AWS_RegionEndpoint")?.Value;
                if (string.IsNullOrEmpty(regionEndPoint))
                {
                    throw new EntityNotFoundException(_localizer.GetString("AwsRegionEndPointNotFound"));
                }
                var fileBucket = settings.FirstOrDefault(x => x.Key == "AWS_FileBucket")?.Value;
                var videoBucket = settings.FirstOrDefault(x => x.Key == "AWS_VideoBucket")?.Value;
                var cloudFront = settings.FirstOrDefault(x => x.Key == "AWS_CloudFront")?.Value;
                return new AmazonSettingDto
                {
                    AccessKey = accessKey,
                    SecretKey = secretKey,
                    RegionEndpoint = regionEndPoint,
                    FileBucket = fileBucket,
                    VideoBucket = videoBucket,
                    CloudFront = cloudFront
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to get the aws credential.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredGettingAWsCredential"));
            }
        }

        #endregion
    }
}