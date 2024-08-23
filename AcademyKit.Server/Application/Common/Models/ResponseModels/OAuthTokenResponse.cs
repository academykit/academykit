namespace AcademyKit.Server.Application.Common.Models.ResponseModels
{
    public class OAuthTokenResponse
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
        public string Refresh_token { get; set; }
        public string Scope { get; set; }
        public string Token_type { get; set; }
        public string Id_token { get; set; }
        public string Error { get; set; }
        public string Error_description { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(Error);
    }
}
