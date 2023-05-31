namespace Lingtren.Api.Controllers
{
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
    using System.IdentityModel.Tokens.Jwt;

    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IValidator<LoginRequestModel> _validator;
        private readonly IValidator<ResetPasswordRequestModel> _resetPasswordValidator;
        private readonly IValidator<ChangePasswordRequestModel> _changePasswordValidator;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;

        public AccountController(
            IUserService userService,
            IValidator<LoginRequestModel> validator,
            IValidator<ResetPasswordRequestModel> resetPasswordValidator,
            IValidator<ChangePasswordRequestModel> changePasswordValidator,
            IStringLocalizer<ExceptionLocalizer> localizer)
        {
            _userService = userService;
            _validator = validator;
            _resetPasswordValidator = resetPasswordValidator;
            _changePasswordValidator = changePasswordValidator;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<UserResponseModel> GetUser()
        {
            var user = await _userService.GetAsync(CurrentUser.Id);
            return new UserResponseModel(user);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var result = await _userService.VerifyUserAndGetToken(model).ConfigureAwait(false);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new CommonResponseModel { Message = result.Message });
            }
            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token))
            {
                return BadRequest(new CommonResponseModel { Message = _localizer.GetString("TokenRequired") });
            }
            var response = await _userService.Logout(model.Token, CurrentUser.Id);
            if (!response)
            {
                return NotFound(new CommonResponseModel { Message = _localizer.GetString("TokenNotMatched") });
            }
            return Ok(new { Message = _localizer.GetString("LogoutSuccess") , Success = true});
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new CommonResponseModel { Message = _localizer.GetString("UserNotFound")});
            }
            await _userService.ResetPasswordAsync(user).ConfigureAwait(false);
            return Ok(new { message = _localizer.GetString("ForgetPasswordExecuted"), success = true });
        }

        [HttpPost("VerifyResetToken")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenModel model)
        {
            var passwordResetToken = await _userService.VerifyPasswordResetTokenAsync(model).ConfigureAwait(false);
            return Ok(new { message = _localizer.GetString("PasswordResetTokenMatched"), data = passwordResetToken });
        }

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

            var user = await _userService.GetUserByEmailAsync(email.Trim().ToLower());

            if (user == null)
            {
                return BadRequest(new CommonResponseModel { Message = _localizer.GetString("UserNotFound") });
            }
            user.HashPassword = _userService.HashPassword(model.NewPassword);
            await _userService.UpdateAsync(user, includeProperties: false);
            return Ok(new { message = _localizer.GetString("PasswordResetSuccess"), success = true });
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestModel model)
        {
            var response = await _userService.RefreshTokenAsync(model.Token);
            if (response.IsAuthenticated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(new CommonResponseModel { Message = response.Message });
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel model)
        {
            await _changePasswordValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            await _userService.ChangePasswordAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new { message = _localizer.GetString("PasswordChanged"), success = true });
        }
    }
}
