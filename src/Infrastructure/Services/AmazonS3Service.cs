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
                    ContentType ="video/mp4",
                    FilePath = dto.FilePath
                };
                request.BucketName = dto.Type == MediaType.File ? dto.Setting?.FileBucket : dto.Setting?.VideoBucket;
                await transferUtility.UploadAsync(request);
                return $"{dto.Setting?.CloudFront}/{dto.Key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
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
                    ContentType =dto.File.ContentType,
                    InputStream = dto.File.OpenReadStream(),
                };
                request.BucketName = dto.Type == MediaType.File ? dto.Setting?.FileBucket : dto.Setting?.VideoBucket;
                await transferUtility.UploadAsync(request);
                return $"{dto.Setting?.CloudFront}/{dto.Key}";
         }
         catch (Exception ex)
         {
            _logger.LogError(ex.Message, ex);
             throw ex is ServiceException ? ex : new ServiceException(ex.Message);
         }
       }
    }
}