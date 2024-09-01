using AcademyKit.Server.Application.Common.Models.ResponseModels;

namespace AcademyKit.Server.Application.Common.Interfaces;

public interface IMicrosoftService
{
    /// <summary>
    /// Retrieves the details of the authenticated Microsoft user using the provided access token.
    /// </summary>
    /// <param name="accessToken">The OAuth 2.0 access token for the Microsoft Graph API.</param>
    /// <returns>A <see cref="OAuthUserResponseModel"/> object containing the user's details.</returns>
    /// <exception cref="Exception">Thrown when the request to the Microsoft Graph API fails or an error occurs during processing.</exception>
    Task<OAuthUserResponseModel> GetMicrosoftUserDetails(string accessToken);
}
