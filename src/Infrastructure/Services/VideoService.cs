using FFmpeg.NET;
using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Domain.Entities;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Localization;
using Microsoft.Extensions.Localization;
using System.IO;
using System.Runtime.InteropServices;

namespace Lingtren.Infrastructure.Services
{
    public class VideoService : IVideoService
    {

        public VideoService()
        {

        }

        /// <summary>
        /// Returns the path of executable file of FFMPEG based on current OS platform
        /// </summary>
        /// <value>The executable file path of ffmpeg.</value>
        private string GetFFMpegPath
        {
            get
            {
                //return @"C:\ffmpeg\ffmpeg.exe";
                 return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "/opt/homebrew/bin/ffmpeg" : "/usr/bin/ffmpeg";
            }
        }

        /// <summary>
        /// Handle to get video duration
        /// </summary>
        /// <param name="videoKey"> the video key </param>
        /// <returns> the video duration in total seconds </returns>
        public async Task<int> GetVideoDuration(string videoPath)
        {

            var ffmpeg = new Engine(GetFFMpegPath);
            var data = await ffmpeg.GetMetaDataAsync(new InputFile(videoPath), default).ConfigureAwait(false);
            return Convert.ToInt32(data.Duration.TotalSeconds);
        }

        /// <summary>
        /// Handle to delete tepmp file 
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
