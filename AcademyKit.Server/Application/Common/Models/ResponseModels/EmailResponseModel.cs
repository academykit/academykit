using Newtonsoft.Json;

namespace AcademyKit.Server.Application.Common.Models.ResponseModels
{
    public class EmailResponseModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }
    }
}
