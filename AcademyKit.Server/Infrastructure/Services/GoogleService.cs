using System.Net.Http.Headers;
using AcademyKit.Server.Application.Common.Interfaces;
using AcademyKit.Server.Application.Common.Models.ResponseModels;
using Newtonsoft.Json;

namespace AcademyKit.Server.Infrastructure.Services
{
    public class GoogleService : IGoogleService
    {
        public async Task<string> GetGoogleUserEmail(string accessToken)
        {
            var url = $"https://www.googleapis.com/oauth2/v2/userinfo?fields=email";
            string json;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Bearer",
                        accessToken
                    );
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        json = await response.Content.ReadAsStringAsync();
                        var emailResponse = JsonConvert.DeserializeObject<EmailResponseModel>(json);
                        return emailResponse.Email;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve user email.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user email.", ex);
            }
        }
    }
}
