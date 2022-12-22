namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    public interface IAmazonS3Service
    {
        /// <summary>
        /// Handle to save file to s3 bucket
        /// </summary>
        /// <param name="dto"> the instance of <see cref="AwsS3FileDto" /> .</param>
        /// <returns> the file url </returns>
        Task<string> SaveFileS3BucketAsync(AwsS3FileDto dto);

        /// <summary>
        /// Handle to save recording file to s3 bucket
        /// </summary>
        /// <param name="dto"> the instance of <see cref="AwsS3FileDto" /> .</param>
        /// <returns> the video url </returns>
        Task<string> SaveRecordingFileS3BucketAsync(AwsS3FileDto dto);
    }
}