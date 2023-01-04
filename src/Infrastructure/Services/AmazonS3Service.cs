namespace Lingtren.Infrastructure.Services
{
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Microsoft.Extensions.Logging;

    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly ILogger<AmazonS3Service> _logger;

        public AmazonS3Service(ILogger<AmazonS3Service> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handle to save file to s3 bucket
        /// </summary>
        /// <param name="file"> the instance of <see cref="AwsS3FileDto" /> .</param>
        /// <returns> the file url </returns>
        public async Task<string> SaveFileS3BucketAsync(AwsS3FileDto dto)
        {
            try
            {
                // need to work on region end point
                var client = new AmazonS3Client(dto.Setting?.AccessKey, dto.Setting?.SecretKey, Amazon.RegionEndpoint.APSouth1);
                var transferUtility = new TransferUtility(client);
                var request = new TransferUtilityUploadRequest
                {
                    Key = dto.Key,
                    //   ContentType =dto.File.ContentType,
                    InputStream = dto.File.OpenReadStream(),
                    BucketName = dto.Type == MediaType.File ? dto.Setting?.FileBucket : dto.Setting?.VideoBucket
                };
                await transferUtility.UploadAsync(request);
                return $"{dto.Setting?.CloudFront}/{dto.Key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to save file in s3 bucket");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to save file in s3 bucket");
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
                var client = new AmazonS3Client(dto.Setting?.AccessKey, dto.Setting?.SecretKey, Amazon.RegionEndpoint.APSouth1);
                var transferUtility = new TransferUtility(client);
                var request = new TransferUtilityUploadRequest
                {
                    Key = dto.Key,
                    FilePath = dto.FilePath,
                    BucketName = dto.Setting.VideoBucket
                };
                await transferUtility.UploadAsync(request);
                return $"{dto.Setting?.CloudFront}/{dto.Key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to save recording file in s3 bucket.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to save recording file in s3 bucket.");
            }
        }
    }
}