using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using AcademyKit.Api.Common;
using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Helpers;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Infrastructure.Configurations;
using AcademyKit.Infrastructure.Localization;
using AcademyKit.Server.Application.Common.Interfaces;
using AcademyKit.Server.Application.Common.Models.ResponseModels;
using AcademyKit.Server.Infrastructure.Configurations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AcademyKit.Api.Controllers;

/// <summary>
/// Controller for managing account actions such as login, password reset, and password change.
/// </summary>
public class AccountController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IValidator<LoginRequestModel> _validator;
    private readonly IValidator<ResetPasswordRequestModel> _resetPasswordValidator;
    private readonly IValidator<ChangePasswordRequestModel> _changePasswordValidator;
    private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
    private readonly IGoogleService _googleService;
    private readonly IMicrosoftService _microsoftService;
    private readonly GoogleOAuth _googleOAuth;
    private readonly MicrosoftOAuth _microsoftOAuth;
    private readonly AppUrls _appUrls;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="userService">The instance of <see cref="IUserService"/> used for user management.</param>
    /// <param name="validator">The instance of <see cref="IValidator{LoginRequestModel}"/> for validating login requests.</param>
    /// <param name="resetPasswordValidator">The instance of <see cref="IValidator{ResetPasswordRequestModel}"/> for validating password reset requests.</param>
    /// <param name="changePasswordValidator">The instance of <see cref="IValidator{ChangePasswordRequestModel}"/> for validating password change requests.</param>
    /// <param name="localizer">The instance of <see cref="IStringLocalizer{ExceptionLocalizer}"/> for localizing exception messages.</param>
    /// <param name="googleService">The instance of <see cref="IGoogleService"/> for Google authentication.</param>
    /// <param name="microsoftService">The instance of <see cref="IMicrosoftService"/> for Microsoft authentication.</param>
    /// <param name="googleOAuth">The configuration options for Google OAuth as an instance of <see cref="GoogleOAuth"/>.</param>
    /// <param name="microsoftOAuth">The configuration options for Microsoft OAuth as an instance of <see cref="MicrosoftOAuth"/>.</param>
    /// <param name="appUrls">The configuration options for application urls as an instance of <see cref="AppUrls"/>.</param>
    public AccountController(
        IUserService userService,
        IValidator<LoginRequestModel> validator,
        IValidator<ResetPasswordRequestModel> resetPasswordValidator,
        IValidator<ChangePasswordRequestModel> changePasswordValidator,
        IStringLocalizer<ExceptionLocalizer> localizer,
        IGoogleService googleService,
        IMicrosoftService microsoftService,
        IOptions<GoogleOAuth> googleOAuth,
        IOptions<MicrosoftOAuth> microsoftOAuth,
        IOptions<AppUrls> appUrls
    )
    {
        _userService = userService;
        _validator = validator;
        _resetPasswordValidator = resetPasswordValidator;
        _changePasswordValidator = changePasswordValidator;
        _localizer = localizer;
        _googleService = googleService;
        _microsoftService = microsoftService;
        _googleOAuth = googleOAuth.Value;
        _microsoftOAuth = microsoftOAuth.Value;
        _appUrls = appUrls.Value;
    }

    /// <summary>
    /// get users
    /// </summary>
    /// <returns> the list of <see cref="UserResponseModel" />. </returns>
    [HttpGet]
    public async Task<UserResponseModel> GetUser()
    {
        var user = await _userService.GetAsync(CurrentUser.Id);
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
        var result = await _userService.VerifyUserAndGetToken(model).ConfigureAwait(false);
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

        var response = await _userService.Logout(model.Token, CurrentUser.Id);
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
        var user = await _userService.GetUserByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest(
                new CommonResponseModel { Message = _localizer.GetString("UserNotFound") }
            );
        }

        await _userService.ResetPasswordAsync(user).ConfigureAwait(false);
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
    ) => await _userService.VerifyPasswordResetTokenAsync(model).ConfigureAwait(false);

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

        var user = await _userService.GetUserByEmailAsync(email.Trim().ToLower());

        if (user == null)
        {
            return BadRequest(
                new CommonResponseModel { Message = _localizer.GetString("UserNotFound") }
            );
        }

        user.HashPassword = _userService.HashPassword(model.NewPassword);
        await _userService.UpdateAsync(user, includeProperties: false);
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
        await _userService.ChangePasswordAsync(model, CurrentUser.Id).ConfigureAwait(false);
        return Ok(
            new CommonResponseModel
            {
                Message = _localizer.GetString("PasswordChanged"),
                Success = true
            }
        );
    }

    /// <summary>
    /// Sign in with Google
    /// </summary>
    /// <returns>The Google sign-in interface</returns>
    [HttpGet("signin-with-google")]
    [AllowAnonymous]
    public IActionResult SignInWithGoogle()
    {
        if (
            string.IsNullOrWhiteSpace(_googleOAuth.ClientId)
            || string.IsNullOrWhiteSpace(_googleOAuth.ClientSecret)
        )
        {
            return RedirectToErrorPage(
                "Google",
                "Missing Client ID or Client Secret",
                "The Client ID and Client Secret must be configured properly."
            );
        }

        var url =
            $"{_googleOAuth.AuthUrl}"
            + $"response_type=code&"
            + $"client_id={_googleOAuth.ClientId}&"
            + $"scope=openid email profile&"
            + $"redirect_uri={_googleOAuth.RedirectUri}&"
            + $"nonce={CommonHelper.GenerateNonce()}&"
            + $"access_type=offline&"
            + $"prompt=consent";

        return Redirect(url);
    }

    /// <summary>
    /// Get Google access token
    /// </summary>
    /// <param name="code">The instance of <see cref="string"/></param>
    /// <returns>The access token</returns>
    [HttpGet("oauth/google/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGoogleAccessToken([FromQuery] string code)
    {
        if (
            string.IsNullOrWhiteSpace(_googleOAuth.ClientId)
            || string.IsNullOrWhiteSpace(_googleOAuth.ClientSecret)
        )
        {
            return RedirectToErrorPage(
                "Google",
                "Missing Client ID or Client Secret",
                "The Client ID and Client Secret must be configured properly."
            );
        }

        var dicData = new Dictionary<string, string>
        {
            { "client_id", _googleOAuth.ClientId },
            { "client_secret", _googleOAuth.ClientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", _googleOAuth.RedirectUri },
            { "access_type", "offline" }
        };

        try
        {
            using var client = new HttpClient();
            using var content = new FormUrlEncodedContent(dicData);
            var response = await client.PostAsync(_googleOAuth.AccessTokenUrl, content);
            var json = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);
            if (tokenResponse.IsSuccess)
            {
                var user = await _googleService.GetGoogleUserDetails(tokenResponse.AccessToken);
                var authenticationModel = await _userService.GenerateTokenUsingGoogleSSOAsync(user);

                return Redirect(
                    $"{_appUrls.Client}/redirect/signIn?userId={authenticationModel.UserId}&token={authenticationModel.Token}&refresh={authenticationModel.RefreshToken}"
                );
            }
            else
            {
                return RedirectToErrorPage(
                    "Google",
                    tokenResponse.Error,
                    tokenResponse.ErrorDescription
                );
            }
        }
        catch (Exception ex)
        {
            return RedirectToErrorPage(
                "Google",
                "An error occurred while retrieving the access token.",
                ex.Message
            );
        }
    }

    /// <summary>
    /// Sign in with Microsoft
    /// </summary>
    /// <returns>The Microsoft login interface</returns>
    [HttpGet("signin-with-microsoft")]
    [AllowAnonymous]
    public IActionResult SignInWithMicrosoft()
    {
        if (
            string.IsNullOrWhiteSpace(_microsoftOAuth.ClientId)
            || string.IsNullOrWhiteSpace(_microsoftOAuth.ClientSecret)
        )
        {
            return RedirectToErrorPage(
                "Microsoft",
                "Missing Client ID or Client Secret",
                "The Client ID and Client Secret must be configured properly."
            );
        }

        var nonce = CommonHelper.GenerateNonce();
        var url =
            $"{_microsoftOAuth.AuthUrl}"
            + $"client_id={_microsoftOAuth.ClientId}&"
            + $"response_type=code&"
            + $"redirect_uri={UrlEncoder.Default.Encode(_microsoftOAuth.RedirectUri)}&"
            + $"response_mode=query&"
            + $"scope=openid email profile User.Read offline_access&"
            + $"nonce={nonce}";

        return Redirect(url);
    }

    /// <summary>
    /// Get Microsoft access token
    /// </summary>
    /// <param name="code">The instance of <see cref="string"/></param>
    /// <returns>The access token</returns>
    [HttpGet("oauth/entraId/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMicrosoftAccessToken(string code)
    {
        if (
            string.IsNullOrWhiteSpace(_microsoftOAuth.ClientId)
            || string.IsNullOrWhiteSpace(_microsoftOAuth.ClientSecret)
        )
        {
            return RedirectToErrorPage(
                "Microsoft",
                "Missing Client ID or Client Secret",
                "The Client ID and Client Secret must be configured properly."
            );
        }

        var dicData = new Dictionary<string, string>
        {
            { "client_id", _microsoftOAuth.ClientId },
            { "scope", "openid email profile User.Read offline_access" },
            { "code", code },
            { "redirect_uri", _microsoftOAuth.RedirectUri },
            { "grant_type", "authorization_code" },
            { "client_secret", _microsoftOAuth.ClientSecret },
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
            var response = await client.PostAsync(_microsoftOAuth.AccessTokenUrl, content);
            var json = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

            if (tokenResponse.IsSuccess)
            {
                var user = await _microsoftService.GetMicrosoftUserDetails(
                    tokenResponse.AccessToken
                );
                var authenticationModel = await _userService.GenerateTokenUsingMicrosoftSSOAsync(
                    user
                );

                return Redirect(
                    $"{_appUrls.Client}/redirect/signIn?userId={authenticationModel.UserId}&token={authenticationModel.Token}&refresh={authenticationModel.RefreshToken}"
                );
            }
            else
            {
                return RedirectToErrorPage(
                    "Microsoft",
                    tokenResponse.Error,
                    tokenResponse.ErrorDescription
                );
            }
        }
        catch (Exception ex)
        {
            return RedirectToErrorPage(
                "Microsoft",
                "An error occurred while retrieving the access token.",
                ex.Message
            );
        }
    }

    /// <summary>
    /// Redirects to the error page with the specified provider and error details
    /// </summary>
    /// <param name="provider">The OAuth provider (Google or Microsoft)</param>
    /// <param name="error">The error message</param>
    /// <param name="details">Additional error details</param>
    /// <returns>The redirection to the error page</returns>
    private IActionResult RedirectToErrorPage(string provider, string error, string details)
    {
        return Redirect(
            $"{_appUrls.Client}/error/oauth?provider={provider}&error={UrlEncoder.Default.Encode(error)}&details={UrlEncoder.Default.Encode(details)}"
        );
    }
}
