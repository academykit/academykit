namespace Lingtren.Application.Common.Interfaces
{
    using Hangfire.Server;
    using Lingtren.Application.Common.Dtos;

    public interface IWebhookService
    {
        /// <summary>
        /// Handle to upload zoom recording
        /// </summary>
        /// <param name="dto"> the instance of <see cref="ZoomRecordPayloadDto" />. </param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
        /// <returns> the task complete </returns>
        Task UploadZoomRecordingAsync(ZoomRecordPayloadDto dto,PerformContext context = null);
    }
}