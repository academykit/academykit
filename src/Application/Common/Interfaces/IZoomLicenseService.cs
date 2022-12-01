namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;

    public interface IZoomLicenseService : IGenericService<ZoomLicense, ZoomLicenseBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to get active available zoom license with in given time period
        /// </summary>
        /// <param name="startDateTime">meeting start date</param>
        /// <param name="duration">meeting duration</param>
        /// <returns></returns>
        Task<IList<ZoomLicenseResponseModel>> GetActiveLicenses(DateTime startDateTime, int duration);

        /// <summary>
        /// Handle to create zoom meeting
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="room"></param>
        /// <param name="zoomLicense"></param>
        /// <returns></returns>
        Task CreateZoomMeetingAsync(Lesson lesson);

        /// <summary>
        /// Create zoom meeting
        /// </summary>
        /// <param name="meetingName">the meeting name</param>
        /// <param name="duration">the duration</param>
        /// <param name="startDate">the start date</param>
        /// <param name="hostEmail">the host email</param>
        /// <returns>the meeting id and passcode and the instance of <see cref="ZoomLicense"/></returns>
        Task<(string, string)> CreateMeetingAsync(string meetingName, int duration, DateTime startDate, string hostEmail);

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
        Task<string> GenerateZoomSignatureAsync(string meetingNumber, bool isHost, int accessTokenValidityInMinutes = 120);

        /// <summary>
        /// Get zoom ZAK Token
        /// </summary>
        /// <param name="hostId">the zoom host id</param>
        /// <returns></returns>
        Task<string> GetZAKAsync(string hostId);
    }
}
