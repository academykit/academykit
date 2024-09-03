using System.Net.Http.Headers;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.ResponseModels;
using Newtonsoft.Json;

namespace AcademyKit.Infrastructure.Services;

/// <summary>
/// Service to interact with Microsoft Graph API.
/// </summary>
public class MicrosoftService : IMicrosoftService
{
    private const string url = "https://graph.microsoft.com/v1.0/me";

    /// <summary>
    /// Retrieves the details of the authenticated Microsoft user using the provided access token.
    /// </summary>
    /// <param name="accessToken">The OAuth 2.0 access token for the Microsoft Graph API.</param>
    /// <returns>A <see cref="OAuthUserResponseModel"/> object containing the user's details.</returns>
    /// <exception cref="Exception">Thrown when the request to the Microsoft Graph API fails or an error occurs during processing.</exception>
    public async Task<OAuthUserResponseModel> GetMicrosoftUserDetails(string accessToken)
    {
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
                    var json = await response.Content.ReadAsStringAsync();
                    var userDetail = JsonConvert.DeserializeObject<MicrosoftUserResponseModel>(
                        json
                    );

                    return new OAuthUserResponseModel
                    {
                        Email = userDetail.Mail,
                        FirstName = userDetail.GivenName,
                        LastName = userDetail.Surname,
                        MobilePhone = userDetail.MobilePhone,
                    };
                }
                else
                {
                    throw new Exception("Failed to retrieve user details.");
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the user details.", ex);
        }
    }
}
