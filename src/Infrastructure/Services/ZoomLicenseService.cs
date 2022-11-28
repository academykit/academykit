namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq.Expressions;
    using System.Text;

    public class ZoomLicenseService : BaseGenericService<ZoomLicense, ZoomLicenseBaseSearchCriteria>, IZoomLicenseService
    {
        private const string zoomAPIPath = "https://api.zoom.us/v2";
        public ZoomLicenseService(
            IUnitOfWork unitOfWork,
            ILogger<ZoomLicenseService> logger) : base(unitOfWork, logger)
        {
        }

        #region Protected Methods
        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<ZoomLicense, bool>> ConstructQueryConditions(Expression<Func<ZoomLicense, bool>> predicate, ZoomLicenseBaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.LicenseEmail.ToLower().Trim().Contains(search)
                || x.HostId.Contains(search));
            }
            if (criteria.IsActive != null)
            {
                predicate.And(p => p.IsActive == criteria.IsActive);
            }
            return predicate;
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(ZoomLicenseBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(ZoomLicense.CreatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<ZoomLicense, object> IncludeNavigationProperties(IQueryable<ZoomLicense> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<ZoomLicense, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }
        #endregion Protected Methods

        /// <summary>
        /// Handle to get active available zoom license with in given time period
        /// </summary>
        /// <param name="startDateTime">meeting start date</param>
        /// <param name="duration">meeting duration</param>
        /// <returns></returns>
        public async Task<IList<ZoomLicenseResponseModel>> GetActiveLicenses(DateTime startDateTime, int duration)
        {
            var data = from zoomLicense in _unitOfWork.DbContext.ZoomLicenses.ToList()
                       join meeting in _unitOfWork.DbContext.Meetings.ToList() on zoomLicense.Id equals meeting.ZoomLicenseId
                       where meeting.StartDate.HasValue && zoomLicense.IsActive &&
                       (meeting.StartDate.Value.AddSeconds(meeting.Duration) < startDateTime || meeting.StartDate.Value > startDateTime.AddSeconds(duration))
                       group meeting by zoomLicense into g
                       select new
                       {
                           g.Key.Id,
                           g.Key.HostId,
                           g.Key.Capacity,
                           g.Key.LicenseEmail,
                           g.Key.IsActive,
                           Count = g.Count()
                       };
            var response = data.Where(x => x.Count < 2).Select(x => new ZoomLicenseResponseModel
            {
                Id = x.Id,
                HostId = x.HostId,
                Capacity = x.Capacity,
                LicenseEmail = x.LicenseEmail,
                IsActive = x.IsActive,
            }).ToList();
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Handle to create zoom meeting
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="room"></param>
        /// <param name="zoomLicense"></param>
        /// <returns></returns>
        public async Task CreateZoomMeetingAsync(Lesson lesson)
        {
            var currentTimeStamp = DateTime.UtcNow;

            var (meetingId, passcode) = await CreateMeetingAsync(lesson.Name, lesson.Meeting.Duration,
                                                                    currentTimeStamp, lesson.Meeting.ZoomLicense.LicenseEmail).ConfigureAwait(false);

            lesson.Meeting.MeetingNumber = long.Parse(meetingId);
            lesson.Meeting.PassCode = passcode;

            //var roomMeeting = new RoomMeeting
            //{
            //    Id = Guid.NewGuid(),
            //    RoomId = room.Id,
            //    MeetingId = Convert.ToInt64(meetingId),
            //    CreatedOn = currentTimeStamp
            //};

            _unitOfWork.GetRepository<Meeting>().Update(lesson.Meeting);
            //await _unitOfWork.GetRepository<RoomMeeting>().InsertAsync(roomMeeting).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }


        #region Zoom Api Service

        /// <summary>
        /// Create zoom meeting
        /// </summary>
        /// <param name="meetingName">the meeting name</param>
        /// <param name="duration">the duration</param>
        /// <param name="startDate">the start date</param>
        /// <param name="hostEmail">the host email</param>
        /// <returns>the meeting id and passcode and the instance of <see cref="ZoomLicense"/></returns>
        public async Task<(string, string)> CreateMeetingAsync(string meetingName, int duration, DateTime startDate, string hostEmail)
        {
            var tokenString = GetZoomJWTAccessToken();
            var client = new RestClient($"{zoomAPIPath}/users/{hostEmail}/meetings");
            var request = new RestRequest().AddHeader("Authorization", String.Format("Bearer {0}", tokenString))
                    .AddJsonBody(new
                    {
                        topic = meetingName,
                        duration,
                        start_time = startDate,
                        timezone = "Asia/Kathmandu",
                    });

            var response = await client.PostAsync(request).ConfigureAwait(false);

            int numericStatusCode = (int)response.StatusCode;
            var jObject = JObject.Parse(response.Content);
            var meetingId = (string)jObject["id"];
            var passcode = (string)jObject["password"];
            return (meetingId, passcode);
        }

        /// <summary>
        /// Handle to delete zoom meeting
        /// </summary>
        /// <param name="meetingId">the meeting id</param>
        /// <returns></returns>
        public async Task DeleteZoomMeeting(string meetingId)
        {
            var tokenString = GetZoomJWTAccessToken();
            var id = (long)Convert.ToInt64(meetingId);
            var client = new RestClient($"{zoomAPIPath}/meetings/{id}");
            var request = new RestRequest().AddHeader("Authorization", String.Format("Bearer {0}", tokenString));

            await client.DeleteAsync(request).ConfigureAwait(false);
        }

        private async Task<string> GetZoomJWTAccessToken()
        {
            var zoomSetting = await _unitOfWork.GetRepository<ZoomSetting>().GetFirstOrDefaultAsync().ConfigureAwait(false);
            if (zoomSetting == null)
            {
                throw new EntityNotFoundException("Zoom setting not found");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var currentTimeStamp = DateTime.UtcNow;
            // Zoom api secret
            byte[] symmetricKey = Encoding.ASCII.GetBytes(zoomSetting.ApiSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Zoom api key
                Issuer = zoomSetting.ApiKey,
                // Token expires after 3 min
                Expires = currentTimeStamp.AddMinutes(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        /// <summary>
        /// Generate Zoom signature
        /// </summary>
        /// <param name="meetingNumber"></param>
        /// <param name="isHost"></param>
        /// <param name="accessTokenValidityInMinutes"></param>
        /// <returns></returns>
        public async Task<string> GenerateZoomSignatureAsync(string meetingNumber, bool isHost, int accessTokenValidityInMinutes = 120)
        {
            DateTime now = DateTime.UtcNow;

            var zoomSetting = await _unitOfWork.GetRepository<ZoomSetting>().GetFirstOrDefaultAsync().ConfigureAwait(false);
            if (zoomSetting == null)
            {
                throw new EntityNotFoundException("Zoom setting not found");
            }

            // Get the current epoch timestamp
            int tsNow = (int)(now - new DateTime(1970, 1, 1)).TotalSeconds;
            int tsAccessExp = (int)(now.AddMinutes(accessTokenValidityInMinutes) - new DateTime(1970, 1, 1)).TotalSeconds;

            return CreateToken(zoomSetting.SdkSecret, new JwtPayload
                {
                    { "sdkKey", zoomSetting.SdkKey },
                    { "mn", meetingNumber},
                    { "role", isHost ? 1: 0},
                    { "iat", tsNow },
                    { "exp", tsAccessExp },
                    {"tokenExp", tsAccessExp},
                    { "appKey", zoomSetting.SdkKey }
                });
        }

        /// <summary>
        /// Handle to create token
        /// </summary>
        /// <param name="secret">the zoom sdk secret</param>
        /// <param name="payload">the instance of <see cref="JwtPayload"/></param>
        /// <returns></returns>

        public static string CreateToken(string secret, JwtPayload payload)
        {
            // Create Security key using private key above:
            // Note that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create a Token
            var header = new JwtHeader(credentials);

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return handler.WriteToken(secToken);
        }

        //// <summary>
        /// Get zoom ZAK Token
        /// </summary>
        /// <param name="hostId">the zoom host id</param>
        /// <returns></returns>
        public async Task<string> GetZAKAsync(string hostId)
        {
            var client = new RestClient($"{zoomAPIPath}/users/{hostId}/token?type=zak");

            try
            {
                var tokenString = GetZoomJWTAccessToken();
                var request = new RestRequest().AddHeader("Authorization", string.Format("Bearer {0}", tokenString));
                request.AddHeader("Content-Type", "application/json");
                var response = await client.GetAsync(request).ConfigureAwait(false);
                var jObject = JObject.Parse(response.Content);
                return (string)jObject["token"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }

        }

        #endregion Zoom Api Service
    }
}
