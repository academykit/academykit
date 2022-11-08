namespace Lingtren.Api.Controllers
{
    using Application.Common.Dtos;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text.RegularExpressions;

    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IValidator<LoginRequestModel> _validator;
        public AccountController(
            IUserService userService,
            IValidator<LoginRequestModel> validator)
        {
            _userService = userService;
            _validator = validator;
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
                return BadRequest(new CommonResponseModel { Message = "Token is required." });
            }
            var response = await _userService.Logout(model.Token, CurrentUser.Id);
            if (!response)
            {
                return NotFound(new CommonResponseModel { Message = "Token not matched." });
            }
            return Ok(new { message = "Logout successfully." });
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new CommonResponseModel { Message = "User Not Found" });
            }
            await _userService.ResetPasswordAsync(user).ConfigureAwait(false);
            return Ok(new { message = "Forgot password executed successfully", success = true });
        }

        [HttpPost("VerifyResetToken")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenModel model)
        {
            var passwordResetToken = await _userService.VerifyPasswordResetTokenAsync(model).ConfigureAwait(false);
            return Ok(new { message = "Password Reset Token Matched Successfully", data = passwordResetToken });
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            var lowercase = new Regex("[a-z]+");
            var uppercase = new Regex("[A-Z]+");
            var digit = new Regex("(\\d)+");
            var symbol = new Regex("(\\W)+");
            if (!lowercase.IsMatch(model.NewPassword) || !uppercase.IsMatch(model.NewPassword) || !digit.IsMatch(model.NewPassword) || !symbol.IsMatch(model.NewPassword))
            {
                return BadRequest(new CommonResponseModel { Message = "Password should contains at least one lowercase, one uppercase, one digit and one symbol" });
            }
            var token = new JwtSecurityTokenHandler().ReadJwtToken(model.PasswordChangeToken);

            var email = (string)token.Payload["email"];
            var exp = (long)token.Payload["exp"];

            if (DateTimeOffset.FromUnixTimeSeconds(exp).ToUniversalTime() <= DateTimeOffset.UtcNow)
            {
                return BadRequest(new CommonResponseModel { Message = "Token Expired" });
            }

            var user = await _userService.GetUserByEmailAsync(email.Trim().ToLower());

            if (user == null)
            {
                return BadRequest(new CommonResponseModel { Message = "User Not Found" });
            }
            user.HashPassword = _userService.HashPassword(model.NewPassword);
            await _userService.UpdateAsync(user, includeProperties: false);
            return Ok(true);
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
    }
}
