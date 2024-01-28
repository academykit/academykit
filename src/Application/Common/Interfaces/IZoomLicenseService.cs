namespace Lingtren.Application.Common.Interfaces
{
    using Hangfire.Server;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;

    public interface IZoomLicenseService
        : IGenericService<ZoomLicense, ZoomLicenseBaseSearchCriteria>
    {
        /// <summary>
        /// Handels to get Active LessonID
        /// </summary>
        /// <param name="model">the instance of <see cref="LiveClassLicenseRequestModel"/></param>
        /// <returns>Instance of zoomid <see cref="ZoomLicenseResponseModel"/></returns>
        Task<IList<ZoomLicenseResponseModel>> GetActiveLicensesAsync(
            LiveClassLicenseRequestModel model
        );

        /// <summary>
        /// Handle to create zoom license async
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="ZoomLicense"/></returns>
        Task<ZoomLicense> CreateZoomLicenseAsync(ZoomLicenseRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to update zoom license
        /// </summary>
        /// <param name="id"> the zoom license id </param>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="ZoomLicense"/></returns>
        Task<ZoomLicense> UpdateZoomLicenseAsync(
            Guid id,
            ZoomLicenseRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to create zoom meeting
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="room"></param>
        /// <param name="zoomLicense"></param>
        /// <returns></returns>
        Task CreateZoomMeetingAsync(Lesson lesson);

        /// <summary>
        /// Handle to delete zoom meeting recording
        /// </summary>
        /// <param name="meetingId"> the meeting id </param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> .</param>
        /// <returns> the task complete </returns>
        Task DeleteZoomMeetingRecordingAsync(long meetingId, PerformContext context = null);

        /// <summary>
        /// Create zoom meeting
        /// </summary>
        /// /// <param name="meetingName">the meeting name</param>
        /// <param name="duration">the duration</param>
        /// <param name="startDate">the start date</param>
        /// <param name="hostEmail">the host email</param>
        /// <returns>the meeting id and passcode and the instance of <see cref="ZoomLicense"/></returns>
        Task<(string, string)> CreateMeetingAsync(
            string meetingName,
            int duration,
            DateTime startDate,
            string hostEmail,
            string existingMeetingId
        );

        /// <summary>
        /// Handle to delete zoom meeting
        /// </summary>
        /// <param name="meetingId">the meeting id</param>
        /// <returns></returns>
        Task DeleteZoomMeeting(string meetingId);

        /// <summary>
        /// Generate Zoom signature
        /// </summary>
        /// <param name="meetingNumber">the meeting number</param>
        /// <param name="isHost">the user is host or not</param>
        /// <param name="accessTokenValidityInMinutes">validity of access token in minutes</param>
        /// <returns></returns>
        Task<string> GenerateZoomSignatureAsync(
            string meetingNumber,
            bool isHost,
            int accessTokenValidityInMinutes = 120
        );

        /// <summary>
        /// Get zoom ZAK Token
        /// </summary>
        /// <param name="hostId">the zoom host id</param>
        /// <returns></returns>
        Task<string> GetZAKAsync(string hostId);
    }
}
