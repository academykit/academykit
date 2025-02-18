namespace AcademyKit.Infrastructure.Services
{
    using System;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Helpers;
    using AcademyKit.Infrastructure.Localization;
    using Application.Common.Dtos;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using Minio;
    using Minio.DataModel.Args;

    public class FileServerService : BaseService, IFileServerService
    {
        public FileServerService(
            IUnitOfWork unitOfWork,
            ILogger<FileServerService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Handle to upload file
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file key or url .</returns>
        public async Task<string> UploadFileAsync(MediaRequestModel model)
        {
            try
            {
                var credentials = await GetCredentialAsync().ConfigureAwait(false);
                var minioClient = new MinioClient()
                    .WithEndpoint(credentials.EndPoint)
                    .WithCredentials(credentials.AccessKey, credentials.SecretKey)
                    .WithSSL()
                    .Build();
                var fileName = string.Concat(model.File.FileName.Where(c => !char.IsWhiteSpace(c)));
                var extension = Path.GetExtension(fileName);
                fileName = $"{Guid.NewGuid()}_{fileName}";
                if (!string.IsNullOrWhiteSpace(extension))
                {
                    MimeTypes.TryGetExtension(model.File.ContentType, out extension);
                    fileName = $"{Guid.NewGuid()}{extension}";
                }

                if (model.Type == MediaType.Private)
                {
                    fileName = $"private/{fileName}";
                }

                if (model.Type == MediaType.Public)
                {
                    fileName = $"public/{fileName}";
                }

                var beArgs = new BucketExistsArgs().WithBucket(credentials.Bucket);
                var found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs().WithBucket(credentials.Bucket);
                    await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                var objectArgs = new PutObjectArgs()
                    .WithObject(fileName)
                    .WithBucket(credentials.Bucket)
                    .WithStreamData(model.File.OpenReadStream())
                    .WithContentType(model.File.ContentType)
                    .WithObjectSize(model.File.Length);

                await minioClient.PutObjectAsync(objectArgs).ConfigureAwait(false);
                return model.Type == MediaType.Private
                    ? fileName
                    : $"{credentials.Url}/{credentials.Bucket}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to upload file to the server."
                );
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to upload the file path
        /// </summary>
        /// <param name="filePath"> the file path </param>
        ///
        /// <returns> the new file path </returns>
        public async Task<string> UploadRecordingFileAsync(string filePath, int fileSize)
        {
            try
            {
                var credentials = await GetCredentialAsync().ConfigureAwait(false);
                var minioClient = new MinioClient()
                    .WithEndpoint(credentials.EndPoint)
                    .WithCredentials(credentials.AccessKey, credentials.SecretKey)
                    .WithSSL()
                    .Build();
                var fileName = $"private/{Guid.NewGuid()}.mp4";
                var objectArgs = new PutObjectArgs()
                    .WithObject(fileName)
                    .WithBucket(credentials.Bucket)
                    .WithFileName(filePath)
                    .WithContentType("video/mp4")
                    .WithObjectSize(fileSize);
                await minioClient.PutObjectAsync(objectArgs);
                _logger.LogInformation(fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to upload file to the server."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorOccurredOnUploadFile"));
            }
        }

        /// <summary>
        /// Handle to get file presigned url
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the pre signed url </returns>
        public async Task<string> GetFilePresignedUrl(string key)
        {
            try
            {
                var credentials = await GetCredentialAsync().ConfigureAwait(false);
                var minioClient = new MinioClient()
                    .WithEndpoint(credentials.PresignedUrl)
                    .WithCredentials(credentials.AccessKey, credentials.SecretKey)
                    .WithSSL()
                    .Build();
                var objectArgs = new PresignedGetObjectArgs()
                    .WithObject(key)
                    .WithBucket(credentials.Bucket)
                    .WithExpiry(credentials.ExpiryTime);
                return await minioClient.PresignedGetObjectAsync(objectArgs).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting file presigned url.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("FilePresignedUrlError"));
            }
        }

        /// <summary>
        /// Handle to get file local path async
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the local file path </returns>
        public async Task<string> GetFileLocalPathAsync(string key)
        {
            var credentials = await GetCredentialAsync().ConfigureAwait(false);
            var minioClient = new MinioClient()
                .WithEndpoint(credentials.EndPoint)
                .WithCredentials(credentials.AccessKey, credentials.SecretKey)
                .WithSSL()
                .Build();

            var objectArgs = new PresignedGetObjectArgs()
                .WithObject(key)
                .WithBucket(credentials.Bucket)
                .WithExpiry(credentials.ExpiryTime);

            var fileUrl = await minioClient
                .PresignedGetObjectAsync(objectArgs)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(fileUrl))
            {
                throw new ArgumentException(_localizer.GetString("FileNotFound"));
            }

            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");
            using (var client = new HttpClient())
            {
                await client.DownloadFileTaskAsync(new Uri(fileUrl), filePath).ConfigureAwait(true);
            }

            return filePath;
        }

        /// <summary>
        /// Handle to upload file
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file key or url .</returns>
        public async Task RemoveFileAsync(string key)
        {
            try
            {
                var credentials = await GetCredentialAsync().ConfigureAwait(false);

                var minioClient = new MinioClient()
                    .WithEndpoint(credentials.EndPoint)
                    .WithCredentials(credentials.AccessKey, credentials.SecretKey)
                    .WithSSL()
                    .Build();

                var objectArgs = new RemoveObjectArgs()
                    .WithBucket(credentials.Bucket)
                    .WithObject(key);

                await minioClient.RemoveObjectAsync(objectArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex.Message,
                    "An error occurred while attempting to upload file to the server."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("UploadFileInServerError"));
            }
        }

        #region private

        /// <summary>
        /// Handle to get credential
        /// </summary>
        /// <returns> the instance of <see cref="MinIoDto" /> .</returns>
        private async Task<MinIoDto> GetCredentialAsync()
        {
            try
            {
                var minioDto = new MinIoDto();
                var settings = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetAllAsync(predicate: x => x.Key.StartsWith("Server"))
                    .ConfigureAwait(false);
                var accessKey = settings.FirstOrDefault(x => x.Key == "Server_AccessKey")?.Value;
                if (string.IsNullOrEmpty(accessKey))
                {
                    throw new EntityNotFoundException(
                        _localizer.GetString("ServerAccessKeyNotFound")
                    );
                }

                var secretKey = settings.FirstOrDefault(x => x.Key == "Server_SecretKey")?.Value;
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new EntityNotFoundException(
                        _localizer.GetString("ServerSecretKeyNotFound")
                    );
                }

                var url = settings.FirstOrDefault(x => x.Key == "Server_Url")?.Value;
                if (string.IsNullOrEmpty(url))
                {
                    throw new EntityNotFoundException(_localizer.GetString("ServerUrlNotFound"));
                }

                var bucket = settings.FirstOrDefault(x => x.Key == "Server_Bucket")?.Value;
                if (string.IsNullOrEmpty(bucket))
                {
                    throw new EntityNotFoundException(_localizer.GetString("ServerBucketNotFound"));
                }

                var endPoint = settings.FirstOrDefault(x => x.Key == "Server_EndPoint")?.Value;
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new EntityNotFoundException(
                        _localizer.GetString("ServerEndPointNotFound")
                    );
                }

                var preSigned = settings.FirstOrDefault(x => x.Key == "Server_PresignedUrl")?.Value;
                if (string.IsNullOrEmpty(preSigned))
                {
                    throw new EntityNotFoundException(
                        _localizer.GetString("ServerPreSignedUlrNotFound")
                    );
                }

                var expiryTime = settings
                    .FirstOrDefault(x => x.Key == "Server_PresignedExpiryTime")
                    ?.Value;
                if (string.IsNullOrEmpty(expiryTime))
                {
                    throw new EntityNotFoundException(
                        _localizer.GetString("ServerPreSignedExpiryTimeNotFound")
                    );
                }

                minioDto.AccessKey = accessKey;
                minioDto.SecretKey = secretKey;
                minioDto.PresignedUrl = preSigned;
                minioDto.Url = url;
                minioDto.ExpiryTime = Convert.ToInt32(expiryTime);
                minioDto.EndPoint = endPoint;
                minioDto.Bucket = bucket;
                return minioDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to get the minio credential."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorOccurredOnMinioCredentials"));
            }
        }
        #endregion
    }
}
