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
        Task UploadZoomRecordingAsync(ZoomRecordPayloadDto dto, PerformContext context = null);

        /// <summary>
        /// Handle to save record of participant join meeting
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomPayLoadDto" /> .</param>
        /// <returns> the task complete </returns>
        Task ParticipantJoinMeetingAsync(ZoomPayLoadDto model);

        /// <summary>
        /// Handle to save record of participant leave meeting
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomPayLoadDto" /> .</param>
        /// <returns> the task complete </returns>
        Task ParticipantLeaveMeetingAsync(ZoomPayLoadDto model);
    }
}
