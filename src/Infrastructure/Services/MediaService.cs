namespace Lingtren.Infrastructure.Services
{
    using System.Net.Http.Headers;
    using Amazon.S3.Transfer;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    public class MediaService : BaseService, IMediaService
    {

        private readonly IFileServerService _fileServerService;
        private readonly IAmazonS3Service _amazonService;

        public MediaService(IUnitOfWork unitOfWork,
        ILogger<MediaService> logger, IFileServerService fileServerService,
        IAmazonS3Service amazonService) : base(unitOfWork, logger)
        {
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
                    selectedStorage.Value = model.Type.ToString();
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
                response.IsActive = true;
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
        public async Task<IList<StorageSettingResponseModel>> GetStorageSettingAsync(Guid currentUserId)
        {
            try
            {
                var response = new List<StorageSettingResponseModel>();
                var keyValues = new List<SettingValue>();
                var setting = await _unitOfWork.GetRepository<Setting>().GetFirstOrDefaultAsync(predicate: x => x.Key == "Storage").ConfigureAwait(false);

                var awsSetting = new StorageSettingResponseModel();
                awsSetting.Type = StorageType.AWS;
                awsSetting.Values = await GetStorageTypeValue(StorageType.AWS).ConfigureAwait(false);
                awsSetting.IsActive = Enum.Parse<StorageType>(setting.Value) == StorageType.AWS;
                response.Add(awsSetting);

                var serverSetting = new StorageSettingResponseModel();
                serverSetting.Type = StorageType.Server;
                serverSetting.Values = await GetStorageTypeValue(StorageType.Server).ConfigureAwait(false);
                serverSetting.IsActive = Enum.Parse<StorageType>(setting.Value) == StorageType.Server;
                response.Add(serverSetting);
                return response;
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
        public async Task<string> UploadFileAsync(MediaRequestModel model)
        {
            try
            {
                var storage = await _unitOfWork.GetRepository<Setting>().GetFirstOrDefaultAsync(predicate: p => p.Key == "Storage").ConfigureAwait(false);
                if (string.IsNullOrEmpty(storage.Value))
                {
                    throw new ArgumentException("Storage setting is not configured");
                }
                string url = "";
                var fileKey = $"{Guid.NewGuid()}_{string.Concat(model.File.FileName.Where(c => !char.IsWhiteSpace(c)))}";
                if (Enum.Parse<StorageType>(storage.Value) == StorageType.AWS)
                {
                    var awsSettings = await GetAwsSettings().ConfigureAwait(false);
                    var awsDto = new AwsS3FileDto
                    {
                        Setting = awsSettings,
                        Key = fileKey,
                        File = model.File,
                        Type = model.Type
                    };
                    url = await _amazonService.SaveFileS3BucketAsync(awsDto).ConfigureAwait(false);
                }
                else
                {
                    var serverSettings = await GetServerStorageSettings().ConfigureAwait(false);
                }
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to upload file.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to upload file.");
            }
        }

        /// <summary>
        /// Handle to upload group file
        /// </summary>
        /// <param name="file"> the instance of <see cref="IFormFile" /> .</param>
        /// <returns> the instance of <see cref="GroupFileDto" /> .</returns>
        public async Task<GroupFileDto> UploadGroupFileAsync(IFormFile file)
        {
            try
            {
                var groupFileDto = new GroupFileDto();
                var storage = await _unitOfWork.GetRepository<Setting>().GetFirstOrDefaultAsync(predicate: p => p.Key == "Storage").ConfigureAwait(false);
                if (string.IsNullOrEmpty(storage.Value))
                {
                    throw new ArgumentException("Storage setting is not configured");
                }
                string url = "";
                var fileKey = $"{Guid.NewGuid()}_{string.Concat(file.FileName.Where(c => !char.IsWhiteSpace(c)))}";
                if (Enum.Parse<StorageType>(storage.Value) == StorageType.AWS)
                {
                    var awsSettings = await GetAwsSettings().ConfigureAwait(false);
                    var awsDto = new AwsS3FileDto
                    {
                        Setting = awsSettings,
                        Key = fileKey,
                        File = file,
                        Type = Application.Common.Dtos.MediaType.File
                    };
                    url = await _amazonService.SaveFileS3BucketAsync(awsDto).ConfigureAwait(false);
                }
                else
                {
                    var serverSettings = await GetServerStorageSettings().ConfigureAwait(false);
                }
                groupFileDto.Url = url;
                groupFileDto.Key = fileKey;
                return groupFileDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to upload the recording file
        /// </summary>
        /// <param name="fileUrl"> the file url </param>
        /// <param name="downloadToken"> the download token </param>
        /// <returns> the instance of <see cref="MediaFileDto" /> .</returns>
        public async Task<MediaFileDto> UploadRecordingFileAsync(string fileUrl, string downloadToken)
        {
            try
            {
                var fileDto = new MediaFileDto();
                  var storage = await _unitOfWork.GetRepository<Setting>().GetFirstOrDefaultAsync(predicate: p => p.Key == "Storage").ConfigureAwait(false);
                if (string.IsNullOrEmpty(storage.Value))
                {
                    throw new ArgumentException("Storage setting is not configured");
                }

                var filePath = Path.Combine(Path.GetTempPath(),$"{Guid.NewGuid()}.mp4");
                using(var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",$"{downloadToken}");
                    await client.DownloadFileTaskAsync(new Uri(fileUrl),filePath).ConfigureAwait(false);
                }

                var key = $"{Guid.NewGuid()}.mp4";
                string url = "";
                if (Enum.Parse<StorageType>(storage.Value) == StorageType.AWS)
                {
                    var awsSettings = await GetAwsSettings().ConfigureAwait(false);
                    var awsDto = new AwsS3FileDto
                    {
                        Setting = awsSettings,
                        Key = key,
                        FilePath = filePath,
                        Type = Application.Common.Dtos.MediaType.Video
                    };
                    url = await _amazonService.SaveRecordingFileS3BucketAsync(awsDto).ConfigureAwait(false);
                    DeleteFilePath(filePath);
                }
                else
                {
                    var serverSettings = await GetServerStorageSettings().ConfigureAwait(false);
                }
                fileDto.Key = key;
                fileDto.Url = url;
                return fileDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        #region private

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

        /// <summary>
        /// Handle to get aws settings
        /// </summary>
        /// <returns> the instance of <see cref="AmazonSettingModel" /> .</returns>
        private async Task<AmazonSettingModel> GetAwsSettings()
        {
            try
            {
                var settings = await _unitOfWork.GetRepository<Setting>().GetAllAsync(predicate: x => x.Key.StartsWith("AWS")).ConfigureAwait(false);
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
                var fileBucket = settings.FirstOrDefault(x => x.Key == "AWS_FileBucket")?.Value;
                var videoBucket = settings.FirstOrDefault(x => x.Key == "AWS_VideoBucket")?.Value;
                var cloudFront = settings.FirstOrDefault(x => x.Key == "AWS_CloudFront")?.Value;
                return new AmazonSettingModel
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
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get server storage setting dto
        /// </summary>
        /// <returns> the instance of <see cref="ServerStorageSettingDto" /> .</returns>
        private async Task<ServerStorageSettingDto> GetServerStorageSettings()
        {
            try
            {
                var serverStorage = new ServerStorageSettingDto();
                var settings = await _unitOfWork.GetRepository<Setting>().GetAllAsync(predicate: x => x.Key.StartsWith("Server")).ConfigureAwait(false);
                var filePath = settings.FirstOrDefault(x => x.Key == "Server_FilePath")?.Value;
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new EntityNotFoundException("Server Storage file path not found");
                }
                var videoPath = settings.FirstOrDefault(x => x.Key == "Server_VideoPath")?.Value;
                if (string.IsNullOrEmpty(videoPath))
                {
                    throw new EntityNotFoundException("Video path not found");
                }

                var serverUrl = settings.FirstOrDefault(x => x.Key == "Server_Url")?.Value;
                var userName = settings.FirstOrDefault(x => x.Key == "Server_UserName")?.Value;
                var password = settings.FirstOrDefault(x => x.Key == "Server_Password")?.Value;

                serverStorage.FilePath = filePath;
                serverStorage.VideoPath = videoPath;
                serverStorage.Url = serverUrl;
                serverStorage.Username = userName;
                serverStorage.Password = password;
                return serverStorage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to delete the file path
        /// </summary>
        /// <param name="filePath"> the file path h</param>
        private void DeleteFilePath(string filePath)
        {
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        #endregion
    }
}
