using System.Net.Http.Headers;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.ResponseModels;
using Newtonsoft.Json;

namespace AcademyKit.Infrastructure.Services;

/// <summary>
/// Service to interact with Google APIs to retrieve user information.
/// </summary>
public class GoogleService : IGoogleService
{
    private const string url = "https://www.googleapis.com/oauth2/v2/userinfo";

    /// <summary>
    /// Retrieves the full details of a Google user using the provided access token.
    /// </summary>
    /// <param name="accessToken">The access token for the Google API.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="OAuthUserResponseModel"/> with the user's details.</returns>
    /// <exception cref="Exception">Thrown when the user details cannot be retrieved.</exception>
    public async Task<OAuthUserResponseModel> GetGoogleUserDetails(string accessToken)
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
                    var userDetail = JsonConvert.DeserializeObject<GoogleUserResponseModel>(json);
                    return new OAuthUserResponseModel
                    {
                        Email = userDetail.Email,
                        FirstName = userDetail.GivenName,
                        LastName = userDetail.FamilyName,
                        ProfilePictureUrl = userDetail.Picture,
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
