namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;

    public interface IAmazonS3Service
    {
        /// <summary>
        /// Handle to upload file to s3 bucket
        /// </summary>
        /// <param name="file"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the instance of <see cref="MediaFileDto" /> . </returns>
        Task<string> UploadFileS3BucketAsync(MediaRequestModel model);

        /// <summary>
        /// Handle to save recording file to s3 bucket
        /// </summary>
        /// <param name="dto"> the instance of <see cref="AwsS3FileDto" /> .</param>
        /// <returns> the video url </returns>
        Task<string> SaveRecordingFileS3BucketAsync(AwsS3FileDto dto);

        /// <summary>
        /// Handle to get s3 presigned file url
        /// </summary>
        /// <param name="key"> the file key </param>
        /// <returns> the pre-signed file url </returns>
        Task<string> GetS3PresignedFileAsync(string key);
    }
}