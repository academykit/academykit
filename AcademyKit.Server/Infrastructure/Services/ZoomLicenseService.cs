namespace AcademyKit.Infrastructure.Services
{
    using System.Data;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq.Expressions;
    using System.Text;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using Hangfire;
    using Hangfire.Server;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json.Linq;
    using RestSharp;

    public class ZoomLicenseService
        : BaseGenericService<ZoomLicense, ZoomLicenseBaseSearchCriteria>,
            IZoomLicenseService
    {
        private const string zoomAPIPath = "https://api.zoom.us/v2";
        private const string zoomOAuthTokenApi = "https://zoom.us/oauth/token";

        public ZoomLicenseService(
            IUnitOfWork unitOfWork,
            ILogger<ZoomLicenseService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        #region Protected Methods
        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<ZoomLicense, bool>> ConstructQueryConditions(
            Expression<Func<ZoomLicense, bool>> predicate,
            ZoomLicenseBaseSearchCriteria criteria
        )
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x =>
                    x.LicenseEmail.ToLower().Trim().Contains(search) || x.HostId.Contains(search)
                );
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
        protected override IIncludableQueryable<ZoomLicense, object> IncludeNavigationProperties(
            IQueryable<ZoomLicense> query
        )
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
        /// Handle to create zoom license async
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="ZoomLicense"/></returns>
        public async Task<ZoomLicense> CreateZoomLicenseAsync(
            ZoomLicenseRequestModel model,
            Guid currentUserId
        )
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var emailExist = await _unitOfWork
                    .GetRepository<ZoomLicense>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.LicenseEmail.ToLower() == model.LicenseEmail.ToLower() && p.IsActive
                    )
                    .ConfigureAwait(false);
                if (emailExist != null)
                {
                    throw new ArgumentException(
                        _localizer.GetString("ZoomLicenseEmailAlreadyUsed")
                    );
                }

                await ValidateZoomLicenseAsync(model.LicenseEmail).ConfigureAwait(false);
                var entity = new ZoomLicense
                {
                    Id = Guid.NewGuid(),
                    LicenseEmail = model.LicenseEmail,
                    HostId = model.HostId,
                    Capacity = model.Capacity,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = currentUserId,
                };
                await _unitOfWork
                    .GetRepository<ZoomLicense>()
                    .InsertAsync(entity)
                    .ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return entity;
            });
        }

        /// <summary>
        /// Handle to update zoom license
        /// </summary>
        /// <param name="id"> the zoom license id </param>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="ZoomLicense"/></returns>
        public async Task<ZoomLicense> UpdateZoomLicenseAsync(
            Guid id,
            ZoomLicenseRequestModel model,
            Guid currentUserId
        )
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var zoomLicenses = await _unitOfWork
                    .GetRepository<ZoomLicense>()
                    .GetAllAsync(predicate: p => p.IsActive)
                    .ConfigureAwait(false);
                var zoomLicense =
                    zoomLicenses.FirstOrDefault(x => x.Id == id)
                    ?? throw new EntityNotFoundException(
                        _localizer.GetString("ZoomLicenseNotFound")
                    );
                var emailAddress = zoomLicenses.Where(x => x.LicenseEmail == model.LicenseEmail);
                if (
                    emailAddress.Any(x =>
                        x.LicenseEmail == model.LicenseEmail && x.Id != zoomLicense.Id
                    )
                )
                {
                    throw new ArgumentException(
                        _localizer.GetString("ZoomLicenseEmailAlreadyUsed")
                    );
                }

                await ValidateZoomLicenseAsync(model.LicenseEmail).ConfigureAwait(false);
                zoomLicense.HostId = model.HostId;
                zoomLicense.LicenseEmail = model.LicenseEmail;
                zoomLicense.Capacity = model.Capacity;
                zoomLicense.UpdatedOn = DateTime.UtcNow;
                zoomLicense.UpdatedBy = currentUserId;
                _unitOfWork.GetRepository<ZoomLicense>().Update(zoomLicense);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return zoomLicense;
            });
        }

        /// <summary>
        /// Handles to get ZoomLicenseId
        /// </summary>
        /// <param name="zoomLicenseIdRequestModel"> the instance of <see cref="LiveClassLicenseRequestModel"/></param>
        /// <returns>the instance of <see cref="ZoomLicenseResponseModel"/></returns>
        public async Task<IList<ZoomLicenseResponseModel>> GetActiveLicensesAsync(
            LiveClassLicenseRequestModel zoomLicenseIdRequestModel
        )
        {
            try
            {
                var zoomLicenses = await _unitOfWork
                    .GetRepository<ZoomLicense>()
                    .GetAllAsync(predicate: p => p.IsActive)
                    .ConfigureAwait(false);
                var startDate = zoomLicenseIdRequestModel.StartDateTime.Date;
                var endTime = zoomLicenseIdRequestModel.StartDateTime.AddMinutes(
                    zoomLicenseIdRequestModel.Duration
                );
                var response = new List<ZoomLicenseResponseModel>();
                var meetingsWithStartDate = await _unitOfWork
                    .GetRepository<Meeting>()
                    .GetAllAsync(predicate: p => p.StartDate.Value.Date == startDate)
                    .ConfigureAwait(false);
                if (meetingsWithStartDate.Count == 0)
                {
                    zoomLicenses.ForEach(x =>
                        response.Add(
                            new ZoomLicenseResponseModel
                            {
                                Id = x.Id,
                                HostId = x.HostId,
                                Capacity = x.Capacity,
                                LicenseEmail = x.LicenseEmail,
                                IsActive = x.IsActive,
                            }
                        )
                    );
                    return response;
                }

                if (!string.IsNullOrEmpty(zoomLicenseIdRequestModel.LessonIdentity))
                {
                    var meeting = await _unitOfWork
                        .GetRepository<Meeting>()
                        .GetFirstOrDefaultAsync(
                            predicate: p =>
                                p.Lesson.Id.ToString() == zoomLicenseIdRequestModel.LessonIdentity
                                || p.Lesson.Slug == zoomLicenseIdRequestModel.LessonIdentity,
                            include: src => src.Include(x => x.Lesson)
                        )
                        .ConfigureAwait(false);
                    if (meetingsWithStartDate.Any(x => x.Id == meeting.Id) == true)
                    {
                        meetingsWithStartDate.Add(meeting);
                    }
                }

                var hasOverlappingMeetings = meetingsWithStartDate.Where(m =>
                    (
                        m.StartDate.HasValue
                        && m.StartDate.Value <= endTime
                        && m.StartDate.Value.AddMinutes(m.Duration / 60)
                            >= zoomLicenseIdRequestModel.StartDateTime
                    )
                );
                if (hasOverlappingMeetings.ToList().Count == 0)
                {
                    zoomLicenses.ForEach(x =>
                        response.Add(
                            new ZoomLicenseResponseModel
                            {
                                Id = x.Id,
                                HostId = x.HostId,
                                Capacity = x.Capacity,
                                LicenseEmail = x.LicenseEmail,
                                IsActive = x.IsActive,
                            }
                        )
                    );
                    return response;
                }

                var filteredZoomLicenses = zoomLicenses.Where(z =>
                    !hasOverlappingMeetings.Any(m => m.ZoomLicenseId == z.Id)
                );
                filteredZoomLicenses.ForEach(x =>
                    response.Add(
                        new ZoomLicenseResponseModel
                        {
                            Id = x.Id,
                            HostId = x.HostId,
                            Capacity = x.Capacity,
                            LicenseEmail = x.LicenseEmail,
                            IsActive = x.IsActive,
                        }
                    )
                );
                if (response.Count == 0)
                {
                    throw new NullReferenceException(_localizer.GetString("AllLicensePlaced"));
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handles to retrieve ZoomID
        /// </summary>
        /// <param name="meetings">the instance of <see cref="Meeting"/></param>
        /// <param name="startDateTime">startDate and Time of the live session</param>
        /// <param name="duration">Duration of live session</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public async Task<List<ZoomLicenseResponseModel>> LessonZoomIdGetAsync(
            IList<Meeting> meetings,
            DateTime startDateTime,
            int duration
        )
        {
            var zoomLicenses = await _unitOfWork
                .GetRepository<ZoomLicense>()
                .GetAllAsync(predicate: p => p.IsActive)
                .ConfigureAwait(false);

            var endTime = startDateTime.AddMinutes(duration);

            var meetingList = meetings.ToList();

            var userMeetings = (
                await _unitOfWork.GetRepository<Meeting>().GetAllAsync().ConfigureAwait(false)
            )
                .AsEnumerable()
                .Where(p =>
                    meetingList.Any(m =>
                        m.StartDate.HasValue
                        && p.StartDate.HasValue
                        && m.StartDate.Value.Date == p.StartDate.Value.Date
                    )
                )
                .ToList();

            var hasOverlappingMeetings = userMeetings.Where(m =>
                (
                    m.StartDate.HasValue
                    && m.StartDate.Value >= startDateTime
                    && m.StartDate.Value < endTime
                )
                || (
                    m.StartDate.HasValue
                    && m.StartDate.Value.AddMinutes(m.Duration) > startDateTime
                    && m.StartDate.Value.AddMinutes(m.Duration) <= endTime
                )
            );

            if (hasOverlappingMeetings.Count() != 0)
            {
                throw new InvalidDataException(_localizer.GetString("TimeSpanAlreadyUsed"));
            }

            var data =
                from zoomLicense in zoomLicenses
                join meeting in userMeetings
                    on zoomLicense.Id equals meeting.ZoomLicenseId
                    into zoomMeeting
                from m in zoomMeeting.DefaultIfEmpty()
                group m by zoomLicense into g
                select new
                {
                    g.Key.Id,
                    g.Key.HostId,
                    g.Key.Capacity,
                    g.Key.LicenseEmail,
                    g.Key.IsActive,
                    Count = g.Count()
                };

            var response = data.Where(x => x.Count < 2)
                .Select(x => new ZoomLicenseResponseModel
                {
                    Id = x.Id,
                    HostId = x.HostId,
                    Capacity = x.Capacity,
                    LicenseEmail = x.LicenseEmail,
                    IsActive = x.IsActive,
                })
                .ToList();

            return response;
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
            var (meetingId, passcode) = await CreateMeetingAsync(
                    lesson.Name,
                    lesson.Meeting.Duration,
                    currentTimeStamp,
                    lesson.Meeting.ZoomLicense.LicenseEmail,
                    lesson.Meeting.MeetingNumber.ToString()
                )
                .ConfigureAwait(false);

            lesson.Meeting.MeetingNumber = long.Parse(meetingId);
            lesson.Meeting.Passcode = passcode;
            _unitOfWork.GetRepository<Meeting>().Update(lesson.Meeting);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to validate zoom license
        /// </summary>
        /// <param name="email"> the email </param>
        private async Task ValidateZoomLicenseAsync(string email)
        {
            try
            {
                var client = new RestClient($"{zoomAPIPath}/users/email");
                var tokenString = await GetOAuthAccessToken().ConfigureAwait(false);
                var request = new RestRequest().AddHeader(
                    "Authorization",
                    string.Format("Bearer {0}", tokenString)
                );
                request.AddQueryParameter("email", email);
                request.AddHeader("Content-Type", "application/json");
                var response = await client.GetAsync(request).ConfigureAwait(false);
                var jObject = JObject.Parse(response.Content);
                var emailExist = (bool)jObject["existed_email"];
                if (!emailExist)
                {
                    throw new ArgumentException(_localizer.GetString("InvalidZoomLicenseEmail"));
                }
            }
            catch (Exception ex)
            {
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
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
        public async Task<(string, string)> CreateMeetingAsync(
            string meetingName,
            int duration,
            DateTime startDate,
            string hostEmail,
            string existingMeetingId
        )
        {
            var tokenString = await GetOAuthAccessToken().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(existingMeetingId))
            {
                var id = (long)Convert.ToInt64(existingMeetingId);
                var clientDelete = new RestClient($"{zoomAPIPath}/meetings/{id}");
                var requestDelete = new RestRequest().AddHeader(
                    "Authorization",
                    string.Format("Bearer {0}", tokenString)
                );

                await clientDelete.DeleteAsync(requestDelete).ConfigureAwait(false);
            }

            var client = new RestClient($"{zoomAPIPath}/users/{hostEmail}/meetings");
            var request = new RestRequest()
                .AddHeader("Authorization", string.Format("Bearer {0}", tokenString))
                .AddJsonBody(
                    new
                    {
                        topic = meetingName,
                        duration = duration / 60,
                        start_time = startDate.ToString("s") + "Z",
                        type = 2,
                        timezone = "Asia/Kathmandu",
                    }
                );

            var response = await client.PostAsync(request).ConfigureAwait(false);
            _ = (int)response.StatusCode;
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
            var tokenString = await GetOAuthAccessToken().ConfigureAwait(false);
            var id = (long)Convert.ToInt64(meetingId);
            var client = new RestClient($"{zoomAPIPath}/meetings/{id}");
            var request = new RestRequest().AddHeader(
                "Authorization",
                string.Format("Bearer {0}", tokenString)
            );

            await client.DeleteAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to delete zoom meeting recording
        /// </summary>
        /// <param name="meetingId"> the meeting id </param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> .</param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task DeleteZoomMeetingRecordingAsync(
            long meetingId,
            PerformContext context = null
        )
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException(_localizer.GetString("ContextNotFound"));
                }

                var tokenString = await GetOAuthAccessToken().ConfigureAwait(false);
                var client = new RestClient(
                    $"{zoomAPIPath}/meetings/{meetingId}/recordings?action=delete"
                );
                var request = new RestRequest().AddHeader(
                    "Authorization",
                    string.Format("Bearer {0}", tokenString)
                );
                await client.DeleteAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message, ex);
            }
        }

        // private async Task<string> GetZoomJWTAccessToken()
        // {
        //     var zoomSetting = await _unitOfWork.GetRepository<ZoomSetting>().GetFirstOrDefaultAsync().ConfigureAwait(false);
        //     if (zoomSetting == null)
        //     {
        //         throw new EntityNotFoundException(_localizer.GetString("ZoomSettingNotFound"));
        //     }
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var currentTimeStamp = DateTime.UtcNow;
        //     // Zoom api secret
        //     byte[] symmetricKey = Encoding.ASCII.GetBytes(zoomSetting.ApiSecret);

        //     var tokenDescriptor = new SecurityTokenDescriptor
        //     {
        //         // Zoom api key
        //         Issuer = zoomSetting.ApiKey,
        //         // Token expires after 3 min
        //         Expires = currentTimeStamp.AddYears(3),
        //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
        //     };

        //     var token = tokenHandler.CreateToken(tokenDescriptor);
        //     var tokenString = tokenHandler.WriteToken(token);
        //     return tokenString;
        // }

        /// <summary>
        /// Handle to get OAuth access token
        /// </summary>
        /// <returns> the access token </returns>
        private async Task<string> GetOAuthAccessToken()
        {
            var zoomSetting = await _unitOfWork
                .GetRepository<ZoomSetting>()
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (zoomSetting == null)
            {
                throw new EntityNotFoundException(_localizer.GetString("ZoomSettingNotFound"));
            }

            var client = new RestClient($"{zoomOAuthTokenApi}");
            var credential = $"{zoomSetting.OAuthClientId}:{zoomSetting.OAuthClientSecret}";
            var credentialByte = Encoding.UTF8.GetBytes(credential);
            var token = Convert.ToBase64String(credentialByte);
            var request = new RestRequest();
            request.AddQueryParameter("grant_type", "account_credentials");
            request.AddQueryParameter("account_id", $"{zoomSetting.OAuthAccountId}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", string.Format("Basic {0}", token));
            var response = await client.PostAsync(request).ConfigureAwait(false);
            var jObject = JObject.Parse(response.Content);
            var accessToken = (string)jObject["access_token"];
            return accessToken;
        }

        /// <summary>
        /// Generate Zoom signature
        /// </summary>
        /// <param name="meetingNumber"></param>
        /// <param name="isHost"></param>
        /// <param name="accessTokenValidityInMinutes"></param>
        /// <returns></returns>
        public async Task<string> GenerateZoomSignatureAsync(
            string meetingNumber,
            bool isHost,
            int accessTokenValidityInMinutes = 120
        )
        {
            var now = DateTime.UtcNow;

            var zoomSetting = await _unitOfWork
                .GetRepository<ZoomSetting>()
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (zoomSetting == null)
            {
                throw new EntityNotFoundException(_localizer.GetString("ZoomSettingNotFound"));
            }

            // Get the current epoch time-stamp
            var tsNow = (int)(now - new DateTime(1970, 1, 1)).TotalSeconds;
            var tsAccessExp = (int)
                (
                    now.AddMinutes(accessTokenValidityInMinutes) - new DateTime(1970, 1, 1)
                ).TotalSeconds;

            return CreateToken(
                zoomSetting.SdkSecret,
                new JwtPayload
                {
                    { "sdkKey", zoomSetting.SdkKey },
                    { "mn", meetingNumber },
                    { "role", isHost ? 1 : 0 },
                    { "iat", tsNow },
                    { "exp", tsAccessExp },
                    { "tokenExp", tsAccessExp },
                    { "appKey", zoomSetting.SdkKey }
                }
            );
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

        /// <summary>
        /// Get zoom ZAK Token
        /// </summary>
        /// <param name="hostId">the zoom host id</param>
        /// <returns>the zak token</returns>
        public async Task<string> GetZAKAsync(string hostId)
        {
            var client = new RestClient($"{zoomAPIPath}/users/{hostId}/token?type=zak");

            try
            {
                var tokenString = await GetOAuthAccessToken().ConfigureAwait(false);
                var request = new RestRequest().AddHeader(
                    "Authorization",
                    string.Format("Bearer {0}", tokenString)
                );
                request.AddHeader("Content-Type", "application/json");
                var response = await client.GetAsync(request).ConfigureAwait(false);
                var jObject = JObject.Parse(response.Content);
                return (string)jObject["token"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to generate zak token.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("GenerateZakTokenError"));
            }
        }

        #endregion Zoom Api Service
    }
}
