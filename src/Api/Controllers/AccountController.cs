namespace Lingtren.Api.Controllers
{
    using Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text.RegularExpressions;

    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public AccountController(
            IUserService userService,
            IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                string validationErrors = string.Join(",<br/>",
                                                        ModelState.Values.Where(E => E.Errors.Count > 0)
                                                        .SelectMany(E => E.Errors)
                                                        .Select(E => E.ErrorMessage)
                                                        .ToArray());
                return BadRequest(validationErrors);
            }
            var result = await _userService.VerifyUserAndGetToken(model).ConfigureAwait(false);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token))
                return BadRequest(new { message = "Token is required" });

            var response = await _userService.RevokeToken(model.Token);
            if (!response)
                return NotFound(new { message = "Token not found" });
            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var generator = new Random();
            var token = generator.Next(0, 1000000).ToString("D6");
            await _emailService.SendForgetPasswordEmail(user.Email, user.FirstName, token);
            return Ok();
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
                return BadRequest("Password should contains at least one lowercase, one uppercase, one digit and one symbol");
            }
            var token = new JwtSecurityTokenHandler().ReadJwtToken(model.PasswordChangeToken);

            var email = (string)token.Payload["email"];
            var exp = (long)token.Payload["exp"];

            if (DateTimeOffset.FromUnixTimeSeconds(exp).ToUniversalTime() <= DateTimeOffset.UtcNow)
            {
                return BadRequest("Token Expired");
            }

            var user = await _userService.GetUserByEmailAsync(email.Trim().ToLower());

            if (user == null)
            {
                return BadRequest("User not found");
            }
            user.HashPassword = _userService.HashPassword(model.NewPassword);
            await _userService.UpdateUserAsync(user);
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
                return BadRequest(response.Message);
            }
        }
    }
}
