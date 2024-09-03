using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;

namespace AcademyKit.Application.Common.Interfaces;

public interface IUserService : IGenericService<User, UserSearchCriteria>
{
    /// <summary>
    /// Verifies user credentials and generates an authentication token.
    /// </summary>
    /// <param name="model">The login request model containing user credentials.</param>
    /// <returns>An authentication model containing the token and user information.</returns>
    Task<AuthenticationModel> VerifyUserAndGetToken(LoginRequestModel model);

    /// <summary>
    /// Generates an authentication token using SSO (Single Sign-On) information.
    /// </summary>
    /// <param name="model">The OAuth user response model containing user information.</param>
    /// <returns>An authentication model containing the token and user information.</returns>
    Task<AuthenticationModel> GenerateTokenUsingSSOAsync(OAuthUserResponseModel model);

    /// <summary>
    /// Logs out a user by invalidating their refresh token.
    /// </summary>
    /// <param name="token">The refresh token to invalidate.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    /// <returns>A boolean indicating whether the logout was successful.</returns>
    Task<bool> Logout(string token, Guid currentUserId);

    /// <summary>
    /// Refreshes an authentication token using a refresh token.
    /// </summary>
    /// <param name="token">The refresh token to use for generating a new authentication token.</param>
    /// <returns>An authentication model containing the new token and user information.</returns>
    Task<AuthenticationModel> RefreshTokenAsync(string token);

    /// <summary>
    /// Retrieves the active refresh token for a user.
    /// </summary>
    /// <param name="user">The user to retrieve the refresh token for.</param>
    /// <returns>The active refresh token for the user.</returns>
    Task<RefreshToken> GetActiveRefreshToken(User user);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user with the specified email address.</returns>
    Task<User> GetUserByEmailAsync(string email);

    /// <summary>
    /// Retrieves a list of trainers based on search criteria.
    /// </summary>
    /// <param name="currentUserId">The ID of the current user.</param>
    /// <param name="criteria">The search criteria for trainers.</param>
    /// <returns>A list of trainer response models.</returns>
    Task<IList<TrainerResponseModel>> GetTrainerAsync(
        Guid currentUserId,
        TeacherSearchCriteria criteria
    );

    /// <summary>
    /// Imports users from a CSV file.
    /// </summary>
    /// <param name="file">The CSV file containing user information.</param>
    /// <param name="currentUserId">The ID of the current user performing the import.</param>
    /// <returns>A string message indicating the result of the import operation.</returns>
    Task<string> ImportUserAsync(IFormFile file, Guid currentUserId);

    /// <summary>
    /// Resets a user's password.
    /// </summary>
    /// <param name="user">The user whose password needs to be reset.</param>
    Task ResetPasswordAsync(User user);

    /// <summary>
    /// Verifies a password reset token.
    /// </summary>
    /// <param name="model">The model containing the email and token to verify.</param>
    /// <returns>A verification token response model.</returns>
    Task<VerificationTokenResponseModel> VerifyPasswordResetTokenAsync(VerifyResetTokenModel model);

    /// <summary>
    /// Generates a random password.
    /// </summary>
    /// <param name="length">The length of the password to generate.</param>
    /// <returns>A randomly generated password.</returns>
    Task<string> GenerateRandomPassword(int length);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="model">The model containing the current and new password.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    Task ChangePasswordAsync(ChangePasswordRequestModel model, Guid currentUserId);

    /// <summary>
    /// Initiates a change email request.
    /// </summary>
    /// <param name="model">The model containing the old and new email addresses.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    /// <returns>A change email response model.</returns>
    Task<ChangeEmailResponseModel> ChangeEmailRequestAsync(
        ChangeEmailRequestModel model,
        Guid currentUserId
    );

    /// <summary>
    /// Resend an email for a pending email change request.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the email resend.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    Task ResendEmailAsync(Guid userId, Guid currentUserId);

    /// <summary>
    /// Resend a change email request.
    /// </summary>
    /// <param name="token">The token for the change email request.</param>
    /// <returns>A change email response model.</returns>
    Task<ChangeEmailResponseModel> ResendChangeEmailRequestAsync(string token);

    /// <summary>
    /// Verifies a change email request.
    /// </summary>
    /// <param name="token">The token for the change email request.</param>
    Task VerifyChangeEmailAsync(string token);

    /// <summary>
    /// Removes all refresh tokens for a user.
    /// </summary>
    /// <param name="currentUserId">The ID of the current user.</param>
    Task RemoveRefreshTokenAsync(Guid currentUserId);

    /// <summary>
    /// Retrieves detailed information about a user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve details for.</param>
    /// <returns>A user response model containing detailed user information.</returns>
    Task<UserResponseModel> GetDetailAsync(Guid userId);

    /// <summary>
    /// Retrieves users eligible for course enrollment.
    /// </summary>
    /// <param name="userId">The ID of the current user.</param>
    /// <param name="courseId">The ID of the course.</param>
    /// <returns>A list of user response models.</returns>
    Task<List<UserResponseModel>> GetUserForCourseEnrollment(Guid userId, string courseId);

    /// <summary>
    /// Adds a user to the default group.
    /// </summary>
    /// <param name="userId">The ID of the user to add.</param>
    /// <param name="currentUserId">The ID of the current user performing the action.</param>
    Task AddUserToDefaultGroup(Guid userId, Guid currentUserId);

    /// <summary>
    /// Removes a user from the default group.
    /// </summary>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <param name="CurrentUserId">The ID of the current user performing the action.</param>
    Task RemoveFromDefaultGroup(Guid userId, Guid CurrentUserId);
}
