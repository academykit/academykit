namespace Lingtren.Infrastructure.Services
{
    using Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using System;

    public class FileServerService : BaseService, IFileServerService
    {

        public FileServerService(IUnitOfWork unitOfWork,
        ILogger<FileServerService> logger,
        IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger,localizer)
        {

        }

        /// <summary>
        /// Handle to upload file 
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file key or url .</returns>
        public async Task<string> UploadFileAsync(MediaRequestModel model)
        {
            try
            {
                var credentails = await GetCredentialAsync().ConfigureAwait(false);
                var minio = new Minio.MinioClient().WithEndpoint(credentails.EndPoint).
                            WithCredentials(credentails.AccessKey, credentails.SecretKey).WithSSL().Build();
                var fileName = string.Concat(model.File.FileName.Where(c => !char.IsWhiteSpace(c)));
                var extension = Path.GetExtension(fileName);
                fileName = $"{Guid.NewGuid()}_{fileName}";
                var bucketName = "";
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

                var objectArgs = new Minio.PutObjectArgs().WithObject(fileName).WithBucket(credentails.Bucket).WithStreamData(model.File.OpenReadStream()).
                    WithContentType(model.File.ContentType).WithObjectSize(model.File.Length);
                await minio.PutObjectAsync(objectArgs);
                return model.Type == MediaType.Private ? fileName : $"{credentails.Url}/{credentails.Bucket}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while attempting to upload file to the server.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to upload file to the server.");
            }
        }

         /// <summary>
        /// Handle to upload the file path
        /// </summary>
        /// <param name="filePath"> the file path </param>
        /// 
        /// <returns> the new file path </returns>
        public async Task<string> UploadRecordingFileAsync(string filePath,int fileSize)
        {
            try
            {
                 var credentails = await GetCredentialAsync().ConfigureAwait(false);
                var minio = new Minio.MinioClient().WithEndpoint(credentails.EndPoint).
                            WithCredentials(credentails.AccessKey, credentails.SecretKey).WithSSL().Build();
                var fileName = $"private/{Guid.NewGuid()}.mp4";
                var objectArgs = new Minio.PutObjectArgs().WithObject(fileName).WithBucket(credentails.Bucket).WithFileName(filePath).
                    WithContentType("video/mp4").WithObjectSize(fileSize);
                await minio.PutObjectAsync(objectArgs);
                _logger.LogInformation(fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while attempting to upload file to the server.");
                throw ex is ServiceException ? ex : new ServiceException($"An error occurred while attempting to upload file to the server. {ex.Message}");
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
                var credentails = await GetCredentialAsync().ConfigureAwait(false);
                var minio = new Minio.MinioClient().WithEndpoint(credentails.PresignedUrl).
                            WithCredentials(credentails.AccessKey, credentails.SecretKey).WithSSL().Build();
                var objectArgs = new Minio.PresignedGetObjectArgs().WithObject(key).WithBucket(credentails.Bucket).WithExpiry(credentails.ExpiryTime);
                return await minio.PresignedGetObjectAsync(objectArgs).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while getting file presigned url.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while getting file presigned url.");
            }
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
                var credentails = await GetCredentialAsync().ConfigureAwait(false);
                var minio = new Minio.MinioClient().WithEndpoint(credentails.EndPoint).
                            WithCredentials(credentails.AccessKey, credentails.SecretKey).WithSSL().Build();


                var objectArgs = new Minio.RemoveObjectArgs().WithBucket(credentails.Bucket).WithObject(key);
                await minio.RemoveObjectAsync(objectArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while attempting to upload file to the server.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to upload file to the server.");
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
                var miniodto = new MinIoDto();
                var settings = await _unitOfWork.GetRepository<Setting>().GetAllAsync(predicate: x => x.Key.StartsWith("Server")).ConfigureAwait(false);
                var accessKey = settings.FirstOrDefault(x => x.Key == "Server_AccessKey")?.Value;
                if (string.IsNullOrEmpty(accessKey))
                {
                    throw new EntityNotFoundException("Server Access key not found.");
                }

                var secretKey = settings.FirstOrDefault(x => x.Key == "Server_SecretKey")?.Value;
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new EntityNotFoundException("Server secret key not found.");
                }

                var url = settings.FirstOrDefault(x => x.Key == "Server_Url")?.Value;
                if (string.IsNullOrEmpty(url))
                {
                    throw new EntityNotFoundException("Server url not found.");
                }
                var bucket = settings.FirstOrDefault(x => x.Key == "Server_Bucket")?.Value;
                if (string.IsNullOrEmpty(bucket))
                {
                    throw new EntityNotFoundException("Server bucket not found.");
                }

                var endPoint = settings.FirstOrDefault(x => x.Key == "Server_EndPoint")?.Value;
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new EntityNotFoundException("Server end point not found.");
                }
                var preSigned = settings.FirstOrDefault(x => x.Key == "Server_PresignedUrl")?.Value;
                if (string.IsNullOrEmpty(preSigned))
                {
                    throw new EntityNotFoundException("Server pre-signed url  not found.");
                }
                var expiryTime = settings.FirstOrDefault(x => x.Key == "Server_PresignedExpiryTime")?.Value;
                if (string.IsNullOrEmpty(expiryTime))
                {
                    throw new EntityNotFoundException("Server end point not found.");
                }
                miniodto.AccessKey = accessKey;
                miniodto.SecretKey = secretKey;
                miniodto.PresignedUrl = preSigned;
                miniodto.Url = url;
                miniodto.ExpiryTime = Convert.ToInt32(expiryTime);
                miniodto.EndPoint = endPoint;
                miniodto.Bucket = bucket;
                return miniodto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to get the minio credential.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to get the minio credential.");
            }
        }

        #endregion

    }
}