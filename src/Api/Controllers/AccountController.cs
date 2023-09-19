namespace Lingtren.Api.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using Application.Common.Dtos;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// Controller for managing account actions
    /// </summary>
    public class AccountController : BaseApiController
    {
        private readonly IUserService userService;
        private readonly IValidator<LoginRequestModel> _validator;
        private readonly IValidator<ResetPasswordRequestModel> _resetPasswordValidator;
        private readonly IValidator<ChangePasswordRequestModel> _changePasswordValidator;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;

        /// <summary>
        /// Initializes the new instance of <see cref="AccountController" /> 
        /// </summary>
        /// <param name="userService"> the instance of <see cref="IUserService" /> .</param>
        /// <param name="validator"> the instance of <se cref="IValidator" />  for the instance of <see cref="LoginRequestModel" /> </param>
        /// <param name="resetPasswordValidator"> the instance of <se cref="IValidator" />  for the instance of <see cref="ResetPasswordRequestModel" /> </param>
        /// <param name="changePasswordValidator"> the instance of <se cref="IValidator" />  for the instance of <see cref="ChangePasswordRequestModel" /> </param>
        /// <param name="localizer">the instance of <se cref="IStringLocalizer" />  for the instance of <see cref="ExceptionLocalizer" /> </param>
        public AccountController(
            IUserService userService,
            IValidator<LoginRequestModel> validator,
            IValidator<ResetPasswordRequestModel> resetPasswordValidator,
            IValidator<ChangePasswordRequestModel> changePasswordValidator,
            IStringLocalizer<ExceptionLocalizer> localizer
          )
        {
            this.userService = userService;
            _validator = validator;
            _resetPasswordValidator = resetPasswordValidator;
            _changePasswordValidator = changePasswordValidator;
            _localizer = localizer;
        }

        /// <summary>
        /// get users
        /// </summary>
        /// <returns> the list of <see cref="UserResponseModel" />. </returns>
        [HttpGet]
        public async Task<UserResponseModel> GetUser()
        {
            var user = await userService.GetAsync(CurrentUser.Id);
            return new UserResponseModel(user);
        }

        /// <summary>
        /// login the account
        /// </summary>
        /// <param name="model"> the instance of <see cref="LoginRequestModel"/> </param>
        /// <returns> the task complete </returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var result = await userService.VerifyUserAndGetToken(model).ConfigureAwait(false);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new CommonResponseModel { Message = result.Message });
            }

            return Ok(result);
        }

        /// <summary>
        /// account logout
        /// </summary>
        /// <param name="model"> the instance of <see cref="RefreshTokenRequestModel"/></param>
        /// <returns> the instance of <see cref="CommonResponseModel"/></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token))
            {
                return BadRequest(new CommonResponseModel { Message = _localizer.GetString("TokenRequired") });
            }

            var response = await userService.Logout(model.Token, CurrentUser.Id);
            if (!response)
            {
                return NotFound(new CommonResponseModel { Message = _localizer.GetString("TokenNotMatched") });
            }

            return Ok(new CommonResponseModel { Message = _localizer.GetString("LogoutSuccess"), Success = true });
        }

        /// <summary>
        /// forgot password of account
        /// </summary>
        /// <param name="model"> the instance of <see cref="ForgotPasswordRequestModel"/></param>
        /// <returns> the instance of <see cref="CommonResponseModel"/></returns>
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            var user = await userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new CommonResponseModel { Message = _localizer.GetString("UserNotFound") });
            }

            await userService.ResetPasswordAsync(user).ConfigureAwait(false);
            return Ok(new CommonResponseModel { Message = _localizer.GetString("ForgetPasswordExecuted"), Success = true });
        }

        /// <summary>
        /// verify reset token of account
        /// </summary>
        /// <param name="model"> the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns> the instance of <see cref="VerificationTokenResponseModel"/></returns>
        [HttpPost("VerifyResetToken")]
        [AllowAnonymous]
        public async Task<VerificationTokenResponseModel> VerifyResetToken([FromBody] VerifyResetTokenModel model) => await userService.VerifyPasswordResetTokenAsync(model).ConfigureAwait(false);

        /// <summary>
        /// reset password of account
        /// </summary>
        /// <param name="model"> the instance of <see cref="ResetPasswordRequestModel"/></param>
        /// <returns> the instance of <see cref="CommonResponseModel"/></returns>
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            await _resetPasswordValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(model.PasswordChangeToken);

            var email = (string)token.Payload["email"];
            var exp = (long)token.Payload["exp"];

            if (DateTimeOffset.FromUnixTimeSeconds(exp).ToUniversalTime() <= DateTimeOffset.UtcNow)
            {
                return BadRequest(new CommonResponseModel { Message = _localizer.GetString("TokenExpired") });
            }

            var user = await userService.GetUserByEmailAsync(email.Trim().ToLower());

            if (user == null)
            {
                return BadRequest(new CommonResponseModel { Message = _localizer.GetString("UserNotFound") });
            }

            user.HashPassword = userService.HashPassword(model.NewPassword);
            await userService.UpdateAsync(user, includeProperties: false);
            return Ok(new CommonResponseModel { Message = _localizer.GetString("PasswordResetSuccess"), Success = true });
        }

        /// <summary>
        /// refresh token of account
        /// </summary>
        /// <param name="model"> the instance of <see cref="RefreshTokenRequestModel"/></param>
        /// <returns> the instance of <see cref="CommonResponseModel"/></returns>
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestModel model)
        {
            var response = await userService.RefreshTokenAsync(model.Token);
            if (response.IsAuthenticated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(new CommonResponseModel { Message = response.Message });
            }
        }

        /// <summary>
        /// change password of account
        /// </summary>
        /// <param name="model"> the instance of <see cref="ChangePasswordRequestModel"/></param>
        /// <returns> the instance of <see cref="CommonResponseModel"/></returns>
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel model)
        {
            await _changePasswordValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            await userService.ChangePasswordAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel { Message = _localizer.GetString("PasswordChanged"), Success = true });
        }
    }
}
