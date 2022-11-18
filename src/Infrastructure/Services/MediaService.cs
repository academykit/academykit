namespace Lingtren.Infrastructure.Services
{
    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    public class MediaService : IMediaService
    {
        private readonly ILogger<MediaService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string accessKey;
        private readonly string secretAccessKey;
        private readonly string imageBucket;
        private readonly string cloudFront;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private readonly AmazonS3Client s3Client;
        public MediaService(ILogger<MediaService> logger,
         IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            accessKey = _configuration["Amazon:AccessKey"];
            secretAccessKey = _configuration["Amazon:SecretAccessKey"];
            imageBucket = _configuration["Amazon:ImageBucket"];
            cloudFront = _configuration["Amazon:CloudFront"];
            s3Client = new AmazonS3Client(accessKey, secretAccessKey, bucketRegion);
        }

        /// <summary>
        /// Handle to upload the file
        /// </summary>
        /// <param name="file"> the instance of <see cref="file" /> .</param>
        /// <returns> the file url </returns>
        public async Task<string> UploadFile(IFormFile file)
        {
            try
            {
                var key = $"{Guid.NewGuid()}.png";
                var fileTransferUtility = new TransferUtility(s3Client);
                var fileData = await this.ConvertFileToByte(file).ConfigureAwait(false);
                using (var stream = new MemoryStream(fileData))
                {
                    var request = new TransferUtilityUploadRequest
                    {
                        BucketName = this.imageBucket,
                        Key = key,
                        ContentType = "image/png",
                        InputStream = stream
                    };
                    await fileTransferUtility.UploadAsync(request).ConfigureAwait(false);
                }
                return $"{cloudFront}/{key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to upload the video file
        /// </summary>
        /// <param name="file"> the instance of <see cref="file" /> .</param>
        /// <returns> the file url </returns>
        public async Task<string> UploadVideo(IFormFile file)
        {
            try
            {
                var key = Guid.NewGuid().ToString() + "_" + string.Concat(file.FileName.Where(c => !Char.IsWhiteSpace(c)));
                var transferUtility = new TransferUtility(s3Client);
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = this.imageBucket,
                    Key = key,
                    ContentType = file.ContentType,
                    InputStream = file.OpenReadStream(),
                };
                await transferUtility.UploadAsync(request);
                return $"{cloudFront}/{key}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }


        /// <summary>
        /// handle to convert file to byte
        /// </summary>
        /// <param name="file"> the instance of <see cref="IFormFile" /> .</param>
        /// <returns> task complete </returns>
        private async Task<byte[]> ConvertFileToByte(IFormFile file)
        {

            try
            {
                byte[] fileData = null;
                using (var fileStream = file.OpenReadStream())
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                    fileData = memoryStream.ToArray();
                }
                return fileData;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }
    }
}
