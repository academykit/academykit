namespace AcademyKit.Api.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Encodings.Web;
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Helpers;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Infrastructure.Localization;
    using AcademyKit.Server.Application.Common.Interfaces;
    using AcademyKit.Server.Application.Common.Models.ResponseModels;
    using AcademyKit.Server.Infrastructure.Configurations;
    using Application.Common.Dtos;
    using FluentValidation;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

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
        private readonly IGoogleService _googleService;
        private readonly IMicrosoftService _microsoftService;
        private readonly Google _google;
        private readonly Microsoft _microsoft;

        /// <summary>
        /// Initializes the new instance of <see cref="AccountController" />
        /// </summary>
        /// <param name="userService"> the instance of <see cref="IUserService" /> .</param>
        /// <param name="validator"> the instance of <se cref="IValidator" />  for the instance of <see cref="LoginRequestModel" /> </param>
        /// <param name="resetPasswordValidator"> the instance of <se cref="IValidator" />  for the instance of <see cref="ResetPasswordRequestModel" /> </param>
        /// <param name="changePasswordValidator"> the instance of <se cref="IValidator" />  for the instance of <see cref="ChangePasswordRequestModel" /> </param>
        /// <param name="localizer">the instance of <se cref="IStringLocalizer" />  for the instance of <see cref="ExceptionLocalizer" /> </param>
        /// <param name="google"></param>
        /// <param name="microsoft"></param>
        public AccountController(
            IUserService userService,
            IValidator<LoginRequestModel> validator,
            IValidator<ResetPasswordRequestModel> resetPasswordValidator,
            IValidator<ChangePasswordRequestModel> changePasswordValidator,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IOptions<Google> google,
            IOptions<Microsoft> microsoft,
            IGoogleService googleService,
            IMicrosoftService microsoftService
        )
        {
            this.userService = userService;
            _validator = validator;
            _resetPasswordValidator = resetPasswordValidator;
            _changePasswordValidator = changePasswordValidator;
            _localizer = localizer;
            _google = google.Value;
            _microsoft = microsoft.Value;
            _googleService = googleService;
            _microsoftService = microsoftService;
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
            await _validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
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
                return BadRequest(
                    new CommonResponseModel { Message = _localizer.GetString("TokenRequired") }
                );
            }

            var response = await userService.Logout(model.Token, CurrentUser.Id);
            if (!response)
            {
                return NotFound(
                    new CommonResponseModel { Message = _localizer.GetString("TokenNotMatched") }
                );
            }

            return Ok(
                new CommonResponseModel
                {
                    Message = _localizer.GetString("LogoutSuccess"),
                    Success = true
                }
            );
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
                return BadRequest(
                    new CommonResponseModel { Message = _localizer.GetString("UserNotFound") }
                );
            }

            await userService.ResetPasswordAsync(user).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel
                {
                    Message = _localizer.GetString("ForgetPasswordExecuted"),
                    Success = true
                }
            );
        }

        /// <summary>
        /// verify reset token of account
        /// </summary>
        /// <param name="model"> the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns> the instance of <see cref="VerificationTokenResponseModel"/></returns>
        [HttpPost("VerifyResetToken")]
        [AllowAnonymous]
        public async Task<VerificationTokenResponseModel> VerifyResetToken(
            [FromBody] VerifyResetTokenModel model
        ) => await userService.VerifyPasswordResetTokenAsync(model).ConfigureAwait(false);

        /// <summary>
        /// reset password of account
        /// </summary>
        /// <param name="model"> the instance of <see cref="ResetPasswordRequestModel"/></param>
        /// <returns> the instance of <see cref="CommonResponseModel"/></returns>
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            await _resetPasswordValidator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(model.PasswordChangeToken);

            var email = (string)token.Payload["email"];
            var exp = (long)token.Payload["exp"];

            if (DateTimeOffset.FromUnixTimeSeconds(exp).ToUniversalTime() <= DateTimeOffset.UtcNow)
            {
                return BadRequest(
                    new CommonResponseModel { Message = _localizer.GetString("TokenExpired") }
                );
            }

            var user = await userService.GetUserByEmailAsync(email.Trim().ToLower());

            if (user == null)
            {
                return BadRequest(
                    new CommonResponseModel { Message = _localizer.GetString("UserNotFound") }
                );
            }

            user.HashPassword = userService.HashPassword(model.NewPassword);
            await userService.UpdateAsync(user, includeProperties: false);
            return Ok(
                new CommonResponseModel
                {
                    Message = _localizer.GetString("PasswordResetSuccess"),
                    Success = true
                }
            );
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
            await _changePasswordValidator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            await userService.ChangePasswordAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel
                {
                    Message = _localizer.GetString("PasswordChanged"),
                    Success = true
                }
            );
        }

        /// <summary>
        /// sign in with google
        /// </summary>
        /// <returns>the google sign in interface</returns>
        [HttpGet("signin-with-google")]
        [AllowAnonymous]
        public IActionResult SignInWithGoogle()
        {
            var url =
                $"{_google.AuthUrl}"
                + $"response_type=code&"
                + $"client_id={_google.ClientId}&"
                + $"scope=openid email&"
                + $"redirect_uri={_google.RedirectUri}&"
                + $"nonce={CommonHelper.GenerateNonce}&"
                + $"access_type=offline&"
                + $"prompt=consent";

            return Redirect(url);
        }

        /// <summary>
        /// get access token
        /// </summary>
        /// <param name="code">the instance of <see cref="string"/></param>
        /// <returns>the access token</returns>
        [HttpGet("google/getAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGoogleAccessToken([FromQuery] string code)
        {
            var dicData = new Dictionary<string, string>
            {
                { "client_id", _google.ClientId },
                { "client_secret", _google.ClientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", _google.RedirectUri },
                { "access_type", "offline" }
            };

            try
            {
                using var client = new HttpClient();
                using var content = new FormUrlEncodedContent(dicData);
                var response = await client.PostAsync(_google.AccessTokenUrl, content);
                var json = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);
                if (tokenResponse.IsSuccess)
                {
                    var userEmail = await _googleService.GetGoogleUserEmail(
                        tokenResponse.Access_token
                    );
                    var authenticationModel = await userService.GetTokenForExternalLoginProvider(
                        userEmail
                    );
                    return Ok(new { tokenResponse, authenticationModel });
                }
                else
                {
                    return BadRequest(new { tokenResponse.Error, tokenResponse.Error_description });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "An error occurred while retrieving the access token.",
                        Details = ex.Message
                    }
                );
            }
        }

        /// <summary>
        /// get refresh token of google
        /// </summary>
        /// <param name="refreshToken">the instance of <see cref="string"/></param>
        /// <returns>the refresh token</returns>
        [HttpPost("google/refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGoogleRefreshToken(
            [FromBody] RefreshTokenRequestModel model
        )
        {
            var dicData = new Dictionary<string, string>
            {
                { "client_id", _google.ClientId },
                { "client_secret", _google.ClientSecret },
                { "refresh_token", model.Token },
                { "grant_type", "refresh_token" }
            };
            try
            {
                using var client = new HttpClient();
                using var content = new FormUrlEncodedContent(dicData);
                var response = await client.PostAsync(_google.AccessTokenUrl, content);
                var json = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

                if (tokenResponse.IsSuccess)
                {
                    return Ok(new { tokenResponse.Access_token, tokenResponse.Expires_in });
                }
                else
                {
                    return BadRequest(new { tokenResponse.Error, tokenResponse.Error_description });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "An error occurred while renewing the access token.",
                        Details = ex.Message
                    }
                );
            }
        }

        /// <summary>
        /// sign in with microsoft
        /// </summary>
        /// <returns>the microsoft login interface</returns>
        [HttpGet("signin-with-microsoft")]
        [AllowAnonymous]
        public IActionResult SignInWithMicrosoft()
        {
            var nonce = CommonHelper.GenerateNonce();
            var url =
                $"{_microsoft.AuthUrl}"
                + $"client_id={_microsoft.ClientId}&"
                + $"response_type=code&"
                + $"redirect_uri={UrlEncoder.Default.Encode(_microsoft.RedirectUri)}&"
                + $"response_mode=query&"
                + $"scope=openid email profile User.Read offline_access&"
                + $"nonce={nonce}";

            return Redirect(url);
        }

        /// <summary>
        /// get microsoft access token
        /// </summary>
        /// <param name="code">the instance of <see cref="string"/></param>
        /// <returns>the access token</returns>
        [HttpGet("oauth/entraId/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMicrosoftAccessToken(string code)
        {
            var dicData = new Dictionary<string, string>
            {
                { "client_id", _microsoft.ClientId },
                { "scope", "openid email profile User.Read offline_access" },
                { "code", code },
                { "redirect_uri", _microsoft.RedirectUri },
                { "grant_type", "authorization_code" },
                { "client_secret", _microsoft.ClientSecret },
            };

            try
            {
                using var client = new HttpClient();
                var authHeader = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes("client_id:client_secret")
                );
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    authHeader
                );

                using var content = new FormUrlEncodedContent(dicData);
                var response = await client.PostAsync(_microsoft.AccessTokenUrl, content);
                var json = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

                if (tokenResponse.IsSuccess)
                {
                    var userEmail = await _microsoftService.GetMicrosoftUserEmail(
                        tokenResponse.Access_token
                    );
                    var authenticationModel = await userService.GetTokenForExternalLoginProvider(
                        userEmail
                    );
                    return Redirect($"https://localhost:44414/redirect/signIn?userId={authenticationModel.UserId}&token={tokenResponse.Access_token}&refresh={tokenResponse.Refresh_token}");
                }
                else
                {
                    return BadRequest(new { tokenResponse.Error, tokenResponse.Error_description });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "An error occurred while retrieving the access token.",
                        Details = ex.Message
                    }
                );
            }
        }

        /// <summary>
        /// get microsoft refresh token
        /// </summary>
        /// <param name="refreshToken">the instance of <see cref="string"/></param>
        /// <returns>the microsoft refresh token</returns>
        [HttpPost("microsoft/refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMicrosoftRefreshToken(
            [FromBody] RefreshTokenRequestModel model
        )
        {
            var dicData = new Dictionary<string, string>
            {
                { "client_id", _microsoft.ClientId },
                { "scope", "openid email profile User.Read offline_access" },
                { "refresh_token", model.Token },
                { "grant_type", "refresh_token" },
                { "client_secret", _microsoft.ClientSecret },
            };
            try
            {
                using var client = new HttpClient();
                using var content = new FormUrlEncodedContent(dicData);

                var response = await client.PostAsync(_microsoft.AccessTokenUrl, content);
                var json = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);
                if (tokenResponse.IsSuccess)
                {
                    return Ok(new { tokenResponse.Access_token, tokenResponse.Expires_in });
                }
                else
                {
                    return BadRequest(new { tokenResponse.Error, tokenResponse.Error_description });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "An error occurred while renewing the access token.",
                        Details = ex.Message
                    }
                );
            }
        }
    }
}
