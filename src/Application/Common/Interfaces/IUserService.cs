namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    public interface IUserService : IGenericService<User, UserSearchCriteria>
    {

        /// <summary>
        /// Handle to verified user during login and generate token
        /// </summary>
        /// <param name="tokenRequestDto">the instance of <see cref="TokenRequestDto"/></param>
        /// <returns>the instance of <see cref="AuthenticationModel"/></returns>
        Task<AuthenticationModel> VerifyUserAndGetToken(LoginRequestModel model);
        
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
        Task<RefreshToken?> GetActiveRefreshToken(User user);

        /// <summary>
        /// Handle to get user detail by id
        /// </summary>
        /// <param name="id">the user id</param>
        /// <returns>the instance of <see cref="User"/></returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Handle to hash password
        /// </summary>
        /// <param name="password">the password</param>
        /// <param name="salt"></param>
        /// <param name="needsOnlyHash"></param>
        /// <returns></returns>
        string HashPassword(string password, byte[]? salt = null, bool needsOnlyHash = false);

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
        /// <param name="model">the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns>the password change token</returns>
        Task ResetPasswordAsync(User user);

        /// <summary>
        /// Handle to verify reset token
        /// </summary>
        /// <param name="model">the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns>the password change token</returns>
        Task<string> VerifyPasswordResetTokenAsync(VerifyResetTokenModel model);

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
    }
}
