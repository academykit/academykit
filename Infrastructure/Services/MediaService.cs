﻿namespace AcademyKit.Infrastructure.Services
{
    using System.Net.Http.Headers;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class MediaService : BaseService, IMediaService
    {
        private readonly IFileServerService _fileServerService;
        private readonly IAmazonS3Service _amazonService;
        private readonly IVideoService _videoService;

        public MediaService(
            IUnitOfWork unitOfWork,
            ILogger<MediaService> logger,
            IFileServerService fileServerService,
            IAmazonS3Service amazonService,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IVideoService videoService
        )
            : base(unitOfWork, logger, localizer)
        {
            _amazonService = amazonService;
            _fileServerService = fileServerService;
            _videoService = videoService;
        }

        /// <summary>
        /// Handle to update storage setting
        /// </summary>
        /// <param name="model"> the instance of <see cref="StorageSetting" /> . </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        public async Task<StorageSettingResponseModel> StorageUpdateSettingAsync(
            StorageSettingRequestModel model,
            Guid currentUserId
        )
        {
            await IsSuperAdmin(currentUserId).ConfigureAwait(false);
            var newSettings = new List<Setting>();
            var settings = await _unitOfWork
                .GetRepository<Setting>()
                .GetAllAsync()
                .ConfigureAwait(false);
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
            response.Type = System.Enum.Parse<StorageType>(selectedStorage.Value);
            response.IsActive = true;
            response.Values = await GetStorageTypeValue(response.Type).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Handle to get storage setting
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="StorageSettingResponseModel" /> .</returns>
        public async Task<IList<StorageSettingResponseModel>> GetStorageSettingAsync(
            Guid currentUserId
        )
        {
            try
            {
                var response = new List<StorageSettingResponseModel>();
                var keyValues = new List<SettingValue>();
                var setting = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetFirstOrDefaultAsync(predicate: x => x.Key == "Storage")
                    .ConfigureAwait(false);

                var awsSetting = new StorageSettingResponseModel();
                awsSetting.Type = StorageType.AWS;
                awsSetting.Values = await GetStorageTypeValue(StorageType.AWS)
                    .ConfigureAwait(false);
                awsSetting.IsActive =
                    System.Enum.Parse<StorageType>(setting.Value) == StorageType.AWS;
                response.Add(awsSetting);

                var serverSetting = new StorageSettingResponseModel();
                serverSetting.Type = StorageType.Server;
                serverSetting.Values = await GetStorageTypeValue(StorageType.Server)
                    .ConfigureAwait(false);
                serverSetting.IsActive =
                    System.Enum.Parse<StorageType>(setting.Value) == StorageType.Server;
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
        /// Handle to get file
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the pre-signed url </returns>
        public async Task<string> GetFileAsync(string key)
        {
            try
            {
                var storage = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Key == "Storage")
                    .ConfigureAwait(false);
                if (string.IsNullOrEmpty(storage.Value))
                {
                    throw new ArgumentException(
                        _localizer.GetString("StorageSettingNotConfigured")
                    );
                }

                var url = "";
                if (Enum.Parse<StorageType>(storage.Value) == StorageType.AWS)
                {
                    url = await _amazonService.GetS3PresignedFileAsync(key).ConfigureAwait(false);
                }

                if (Enum.Parse<StorageType>(storage.Value) == StorageType.Server)
                {
                    url = await _fileServerService.GetFilePresignedUrl(key).ConfigureAwait(false);
                }

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to get file.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("FileGetError"));
            }
        }

        /// <summary>
        /// Handle to upload the file
        /// </summary>
        /// <param name="model"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the file key or string </returns>
        public async Task<string> UploadFileAsync(MediaRequestModel model)
        {
            try
            {
                var storage = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Key == "Storage")
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(storage.Value))
                {
                    throw new ArgumentException(
                        _localizer.GetString("StorageSettingNotConfigured")
                    );
                }

                var key = "";
                if (Enum.Parse<StorageType>(storage.Value) == StorageType.AWS)
                {
                    key = await _amazonService.UploadFileS3BucketAsync(model).ConfigureAwait(false);
                }
                else
                {
                    key = await _fileServerService.UploadFileAsync(model).ConfigureAwait(false);
                }

                return key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to upload file.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("UploadFileError"));
            }
        }

        /// <summary>
        /// Handle to upload group file
        /// </summary>
        /// <param name="file"> the instance of <see cref="IFormFile" /> .</param>
        /// <returns> the instance of <see cref="GroupFileDto" /> .</returns>
        public async Task<string> UploadGroupFileAsync(IFormFile file)
        {
            try
            {
                var storage = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Key == "Storage")
                    .ConfigureAwait(false);
                if (string.IsNullOrEmpty(storage.Value))
                {
                    throw new ArgumentException(
                        _localizer.GetString("StorageSettingNotConfigured")
                    );
                }

                var model = new MediaRequestModel { File = file, Type = MediaType.Private };
                if (Enum.Parse<StorageType>(storage.Value) == StorageType.AWS)
                {
                    return await _amazonService
                        .UploadFileS3BucketAsync(model)
                        .ConfigureAwait(false);
                }
                else
                {
                    return await _fileServerService.UploadFileAsync(model).ConfigureAwait(false);
                }
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
        /// <returns> the instance of <see cref="VideoModel"/></returns>
        public async Task<VideoModel> UploadRecordingFileAsync(
            string fileUrl,
            string downloadToken,
            int fileSize
        )
        {
            try
            {
                var fileDto = new MediaFileDto();
                var storage = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Key == "Storage")
                    .ConfigureAwait(false);
                if (string.IsNullOrEmpty(storage.Value))
                {
                    throw new ArgumentException(
                        _localizer.GetString("StorageSettingNotConfigured")
                    );
                }

                var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Bearer",
                        $"{downloadToken}"
                    );
                    await client
                        .DownloadFileTaskAsync(new Uri(fileUrl), filePath)
                        .ConfigureAwait(true);
                }

                var duration = await _videoService.GetVideoDuration(filePath).ConfigureAwait(false);
                var videoPath = "";
                if (Enum.Parse<StorageType>(storage.Value) == StorageType.AWS) { }
                else
                {
                    videoPath = await _fileServerService.UploadRecordingFileAsync(
                        filePath,
                        fileSize
                    );
                    DeleteFilePath(filePath);
                }

                return new VideoModel { VideoUrl = videoPath, Duration = duration };
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
                var awsSettings = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetAllAsync(predicate: x => x.Key.StartsWith("AWS"))
                    .ConfigureAwait(false);
                if (awsSettings.Count != default)
                {
                    response = awsSettings
                        .Select(x => new SettingValue { Key = x.Key, Value = x.Value })
                        .ToList();
                }
            }

            if (type == StorageType.Server)
            {
                var serverSettings = await _unitOfWork
                    .GetRepository<Setting>()
                    .GetAllAsync(predicate: x => x.Key.StartsWith("Server"))
                    .ConfigureAwait(false);
                if (serverSettings.Count != default)
                {
                    response = serverSettings
                        .Select(x => new SettingValue { Key = x.Key, Value = x.Value })
                        .ToList();
                }
            }

            return response;
        }

        /// <summary>
        /// Handle to delete the file path
        /// </summary>
        /// <param name="filePath"> the file path h</param>
        private static void DeleteFilePath(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        #endregion
    }
}
