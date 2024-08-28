namespace AcademyKit.Server.Infrastructure.Configurations
{
    public class MicrosoftOAuth
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string AuthUrl { get; set; }
        public string AccessTokenUrl { get; set; }
    }
}
