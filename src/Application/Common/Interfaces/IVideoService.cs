using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtren.Application.Common.Interfaces
{
    public interface IVideoService
    {
        /// <summary>
        /// Handle to get video duration
        /// </summary>
        /// <param name="videoPath"> the video path </param>
        /// <returns> the video duration in total seconds </returns>
        Task<int> GetVideoDuration(string videoPath);

        /// <summary>
        /// Handle to delete tepmp file 
        /// </summary>
        /// <param name="filePath"> the file path </param>
        void DeleteTempFile(string filePath);
    }
}
