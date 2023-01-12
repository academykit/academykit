namespace Lingtren.Infrastructure.Services
{
    using Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using Minio;

    public class FileServerService : BaseService, IFileServerService
    {

        public FileServerService(IUnitOfWork unitOfWork,
        ILogger<FileServerService> logger) : base(unitOfWork, logger)
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
                var minio = new Minio.MinioClient().WithEndpoint(credentails.Url).
                            WithCredentials(credentails.AccessKey, credentails.SecretKey).Build();
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
                return model.Type == MediaType.Private ? fileName :  $"{credentails.Url}/{credentails.Bucket}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while attempting to upload file to the server.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to upload file to the server.");
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
                var minio = new Minio.MinioClient().WithEndpoint(credentails.Url).
                            WithCredentials(credentails.AccessKey, credentails.SecretKey).Build();
                var objectArgs = new Minio.PresignedGetObjectArgs().WithObject(key).WithBucket(credentails.Bucket).WithExpiry(600);
                var url = await minio.PresignedGetObjectAsync(objectArgs).ConfigureAwait(false);
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting file presigned url.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while getting file presigned url.");
            }
        }

        #region private

        /// <summary>
        /// Handle to get credentails
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
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new EntityNotFoundException("Server url not found.");
                }
                var bucket = settings.FirstOrDefault(x => x.Key == "Server_Bucket")?.Value;
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new EntityNotFoundException("Server bucket not found.");
                }
                miniodto.AccessKey = accessKey;
                miniodto.SecretKey = secretKey;
                miniodto.Url = url;
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