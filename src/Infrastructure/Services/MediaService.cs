namespace Lingtren.Infrastructure.Services
{
    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    public class MediaService : BaseService, IMediaService
    {
        private readonly IConfiguration _configuration;
        private readonly string accessKey;
        private readonly string secretAccessKey;
        private readonly string imageBucket;
        private readonly string cloudFront;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private readonly AmazonS3Client s3Client;
        private readonly IFileServerService _fileServerService;
        private readonly IAmazonService _amazonService;

        public MediaService(IUnitOfWork unitOfWork,
        ILogger<MediaService> logger,IConfiguration configuration,
        IFileServerService fileServerService,IAmazonService amazonService) 
        : base(unitOfWork, logger)
        {
            _configuration = configuration;
            accessKey = _configuration["Amazon:AccessKey"];
            secretAccessKey = _configuration["Amazon:SecretAccessKey"];
            imageBucket = _configuration["Amazon:ImageBucket"];
            cloudFront = _configuration["Amazon:CloudFront"];
            s3Client = new AmazonS3Client(accessKey, secretAccessKey, bucketRegion);
            _amazonService = amazonService;
            _fileServerService = fileServerService;
        }

        /// <summary>
        /// Handle to update storage setting
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageSetting" /> . </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        public async Task<StorageSettingResponseModel> StorageUpdateSettingAsync(StorageSettingRequestModel model, Guid currentUserId)
        {
            try
            {
                await IsSuperAdmin(currentUserId).ConfigureAwait(false);
                var newSettings = new List<Setting>();
                var settings = await _unitOfWork.GetRepository<Setting>().GetAllAsync().ConfigureAwait(false);
                var selectedStorage = settings.FirstOrDefault(x => x.Key == "Storage");
                if (selectedStorage != null)
                {
                    selectedStorage.Value =model.Type.ToString();
                    newSettings.Add(selectedStorage);
                }

                foreach (var item in model.Values)
                {
                    var keyValue = settings.FirstOrDefault(x => x.Key == item.Key);
                    if (keyValue != null)
                    {
                        keyValue.Value = item.Value;
                        newSettings.Add(keyValue);
                    }
                }
                _unitOfWork.GetRepository<Setting>().Update(newSettings);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                var response = new StorageSettingResponseModel();
                response.Type = Enum.Parse<StorageType>(selectedStorage.Value);
                response.Values = await GetStorageTypeValue(response.Type).ConfigureAwait(false);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get storage setting
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        public async Task<StorageSettingResponseModel> GetStorageSettingAsync(Guid currentUserId)
        {
            try
            {
                var response = new StorageSettingResponseModel();
                var keyValues = new List<SettingValue>();
                var setting = await _unitOfWork.GetRepository<Setting>().GetFirstOrDefaultAsync(predicate: x => x.Key == "Storage").ConfigureAwait(false);
                if (setting != null)
                {
                    response.Type = Enum.Parse<StorageType>(setting.Value);
                    response.Values = await GetStorageTypeValue(Enum.Parse<StorageType>(setting.Value)).ConfigureAwait(false);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get setting values
        /// </summary>
        /// <param name="type"> the instance of <see cref="StorageType" />. </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="SettingValue" /> .</returns>
        public async Task<IList<SettingValue>> GetSettingValuesAsync(StorageType type, Guid currentUserId)
        {
            try
            {
                await IsSuperAdmin(currentUserId).ConfigureAwait(false);
                var setting = await _unitOfWork.GetRepository<Setting>().GetFirstOrDefaultAsync(predicate: x => x.Key == "Storage"
                && x.Value == type.ToString()).ConfigureAwait(false);
                if (setting == null)
                {
                    throw new EntityNotFoundException("Storage type not found");
                }
                return await GetStorageTypeValue(type).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
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
                _logger.LogError(ex, "An error occurred while trying to upload file.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to upload file.");
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
                _logger.LogError(ex, "An error occurred while trying to upload video.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to upload video.");
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
                byte[]? fileData = null;
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
                this._logger.LogError(ex, "An error occurred while trying to convert file to byte.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to convert file to byte.");
            }
        }

        /// <summary>
        /// Handle to get storage type value
        /// </summary>
        /// <param name="type"> the instance of <see cref="StorageType" /> .</param>
        /// <returns> the  list of <see cref="SettingValue" />. </returns>
        private async Task<IList<SettingValue>> GetStorageTypeValue(StorageType type)
        {
            var response = new List<SettingValue>();
            if (type == StorageType.AWS)
            {
                var awsSettings = await _unitOfWork.GetRepository<Setting>().GetAllAsync(predicate: x => x.Key.StartsWith("AWS")).ConfigureAwait(false);
                if (awsSettings.Count != default)
                {
                    response = awsSettings.Select(x => new SettingValue
                    {
                        Key = x.Key,
                        Value = x.Value
                    }).ToList();
                }
            }

            if (type == StorageType.Server)
            {
                var serverSettings = await _unitOfWork.GetRepository<Setting>().GetAllAsync(predicate: x => x.Key.StartsWith("Server")).ConfigureAwait(false);
                if (serverSettings.Count != default)
                {
                    response = serverSettings.Select(x => new SettingValue
                    {
                        Key = x.Key,
                        Value = x.Value
                    }).ToList();
                }
            }
            return response;
        }
    }
}
