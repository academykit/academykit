namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Server.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Http;

    public interface IUserService : IGenericService<User, UserSearchCriteria>
    {
        /// <summary>
        /// Handle to verified user during login and generate token
        /// </summary>
        /// <param name="model">the instance of <see cref="LoginRequestModel"/></param>
        /// <returns>the instance of <see cref="AuthenticationModel"/></returns>
        Task<AuthenticationModel> VerifyUserAndGetToken(LoginRequestModel model);

        /// <summary>
        /// Generates an authentication token for a user using SSO information.
        /// </summary>
        /// <param name="model">The SSO user details.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AuthenticationModel"/> containing the token and user information.</returns>
        Task<AuthenticationModel> GenerateTokenUsingSSOAsync(OAuthUserResponseModel model);

        /// <summary>
        /// Handle to logout user and set refresh token false
        /// </summary>
        /// <param name="token">the refresh token</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<bool> Logout(string token, Guid currentUserId);

        /// <summary>
        /// Handle to generate new jwt token from refresh token
        /// </summary>
        /// <param name="token">the refresh token</param>
        /// <returns></returns>
        Task<AuthenticationModel> RefreshTokenAsync(string token);

        /// <summary>
        /// Get single active refresh token of the user
        /// </summary>
        /// <param name="user">the instance of <see cref="User"/></param>
        /// <returns>the instance of <see cref="RefreshToken"/></returns>
        Task<RefreshToken> GetActiveRefreshToken(User user);

        /// <summary>
        /// Handle to get user detail by email
        /// </summary>
        /// <param name="email">the user email</param>
        /// <returns>the instance of <see cref="User"/></returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Handle to hash password
        /// </summary>
        /// <param name="password">the password</param>
        /// <param name="salt"></param>
        /// <param name="needsOnlyHash"></param>
        /// <returns></returns>
        string HashPassword(string password, byte[] salt = null, bool needsOnlyHash = false);

        /// <summary>
        /// Handle to verify password
        /// </summary>
        /// <param name="hashedPasswordWithSalt">the hashed password</param>
        /// <param name="password">the password</param>
        /// <returns></returns>
        bool VerifyPassword(string hashedPasswordWithSalt, string password);

        /// <summary>
        /// Handle to reset password
        /// </summary>
        /// <param name="user">the instance of <see cref="User"/></param>
        /// <returns>the password change token</returns>
        Task ResetPasswordAsync(User user);

        /// <summary>
        /// Handle to verify reset token
        /// </summary>
        /// <param name="model">the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns> the instance of <see cref="VerificationTokenResponseModel"/></returns>
        Task<VerificationTokenResponseModel> VerifyPasswordResetTokenAsync(
            VerifyResetTokenModel model
        );

        /// <summary>
        /// Handle to generate random password
        /// </summary>
        /// <param name="length">the length</param>
        /// <returns></returns>
        Task<string> GenerateRandomPassword(int length);

        /// <summary>
        /// Handle to change user password
        /// </summary>
        /// <param name="model">the instance of <see cref="ChangePasswordRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        Task ChangePasswordAsync(ChangePasswordRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to change user email
        /// </summary>
        /// <param name="model">the instance of <see cref="ChangeEmailRequestModel"/></param>
        /// <returns>the instance of <see cref="ChangeEmailResponseModel"/></returns>
        Task<ChangeEmailResponseModel> ChangeEmailRequestAsync(
            ChangeEmailRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to resend email async
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task ResendEmailAsync(Guid userId, Guid currentUserId);

        /// <summary>
        /// Handle to resend change user email
        /// </summary>
        /// <param name="token">the resend token</param>
        /// <returns>the instance of <see cref="ChangeEmailResponseModel"/></returns>
        Task<ChangeEmailResponseModel> ResendChangeEmailRequestAsync(string token);

        /// <summary>
        /// Handle to verify user email change
        /// </summary>
        /// <param name="token">the token</param>
        /// <returns></returns>
        Task VerifyChangeEmailAsync(string token);

        /// <summary>
        /// Handle to fetch users detail
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>the instance of <see cref="UserResponseModel"/></returns>
        Task<UserResponseModel> GetDetailAsync(Guid userId);

        /// <summary>
        /// Handle to get trainer
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <param name="criteria"> the instance of <see cref="TeacherSearchCriteria"></see></param>
        /// <returns> the list of <see cref="TrainerResponseModel"/></returns>
        Task<IList<TrainerResponseModel>> GetTrainerAsync(
            Guid currentUserId,
            TeacherSearchCriteria criteria
        );

        /// <summary>
        /// Handle to import the user
        /// </summary>
        /// <param name="file"> the instance of <see cref="IFormFile" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task<string> ImportUserAsync(IFormFile file, Guid currentUserId);

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <param name="CourseID">the current course id </param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        Task<List<UserResponseModel>> GetUserForCourseEnrollment(Guid userId, string courseId);

        /// <summary>
        /// Handle to remove the users active refresh token
        /// </summary>
        /// <param name="currentUserId">Current userId</param>
        /// <returns>Task completed</returns>
        Task RemoveRefreshTokenAsync(Guid currentUserId);
        Task AddToDefaultGroup(Guid userId, Guid CurrentUserId);
        Task RemoveFromDefaultGroup(Guid userId, Guid CurrentUserId);
    }
}
