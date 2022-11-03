namespace Lingtren.Api.Common
{
    using Lingtren.Application.Common.Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// This class contains validation methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Represents the JSON serialize settings.
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = "MM/dd/yyyy HH:mm:ss",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Serialized data,</returns>
        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        /// <summary>
        /// De-serialize the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>De-serialized object.</returns>
        public static T? Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, SerializerSettings);
        }

        /// <summary>
        /// Converts to logged in user.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal.</param>
        /// <returns>The logged in user details.</returns>
        public static CurrentUser ToLoggedInUser(this ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.GetClaim("uid", isRequired: true);
            var userName = claimsPrincipal.GetClaim(ClaimTypes.NameIdentifier, isRequired: true);
            var email = claimsPrincipal.GetClaim(ClaimTypes.Email, isRequired: false);
            var mobileNumber = claimsPrincipal.GetClaim("mobile_number", isRequired: false);

            return new CurrentUser { Id = Guid.Parse(userId), Name = userName, Email = email, MobileNumber = mobileNumber, };
        }

        /// <summary>
        /// Gets the claim value.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>The claim value.</returns>
        public static string? GetClaim(this ClaimsPrincipal claimsPrincipal, string claimType, bool isRequired = false)
        {
            Claim claim = FindClaim(claimsPrincipal, claimType, isRequired);

            return claim?.Value;
        }

        /// <summary>
        /// Gets the claim object value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="claimsPrincipal">The claims principal.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>Claim object value.</returns>
        public static T? GetClaim<T>(this ClaimsPrincipal claimsPrincipal, string claimType, bool isRequired = false)
        {
            var claim = FindClaim(claimsPrincipal, claimType, isRequired);
            if (claim == null)
            {
                return default;
            }
            var result = Deserialize<T>(claim.Value);
            return result;
        }

        /// <summary>
        /// Finds the claim.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>Found claim.</returns>
        private static Claim FindClaim(ClaimsPrincipal claimsPrincipal, string claimType, bool isRequired)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
            if (claim == null && isRequired)
            {
                throw new ServiceException($"'{claimType}' claim type is missing.");
            }

            return claim;
        }
    }
}
