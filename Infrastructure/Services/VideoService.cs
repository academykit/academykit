using System.Runtime.InteropServices;
using AcademyKit.Application.Common.Interfaces;
using FFmpeg.NET;

namespace AcademyKit.Infrastructure.Services
{
    public class VideoService : IVideoService
    {
        public VideoService() { }

        /// <summary>
        /// Returns the path of executable file of FFMPEG based on current OS platform
        /// </summary>
        /// <value>The executable file path of ffmpeg.</value>
        private static string GetFFMpegPath =>
            //return @"C:\ffmpeg\ffmpeg.exe";
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? "/opt/homebrew/bin/ffmpeg"
                : "/usr/bin/ffmpeg";

        /// <summary>
        /// Handle to get video duration
        /// </summary>
        /// <param name="videoKey"> the video key </param>
        /// <returns> the video duration in total seconds </returns>
        public async Task<int> GetVideoDuration(string videoPath)
        {
            var ffmpeg = new Engine(GetFFMpegPath);
            var data = await ffmpeg
                .GetMetaDataAsync(new InputFile(videoPath), default)
                .ConfigureAwait(false);
            return Convert.ToInt32(data.Duration.TotalSeconds);
        }

        /// <summary>
        /// Handle to delete temp file
        /// </summary>
        /// <param name="filePath"> the file path </param>
        public void DeleteTempFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        #region private method

        #endregion
    }
}
