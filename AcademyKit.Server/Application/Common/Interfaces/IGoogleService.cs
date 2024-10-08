﻿using AcademyKit.Application.Common.Models.ResponseModels;

namespace AcademyKit.Application.Common.Interfaces;

public interface IGoogleService
{
    /// <summary>
    /// Retrieves the full details of a Google user using the provided access token.
    /// </summary>
    /// <param name="accessToken">The access token for the Google API.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="OAuthUserResponseModel"/> with the user's details.</returns>
    /// <exception cref="Exception">Thrown when the user details cannot be retrieved.</exception>
    Task<OAuthUserResponseModel> GetGoogleUserDetails(string accessToken);
}
