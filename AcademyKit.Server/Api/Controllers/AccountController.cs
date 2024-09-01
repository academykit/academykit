using System.IdentityModel.Tokens.Jwt;
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
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<LoginRequestModel> _validator;
    private readonly IValidator<ResetPasswordRequestModel> _resetPasswordValidator;
    private readonly IValidator<ChangePasswordRequestModel> _changePasswordValidator;
    private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
    private readonly IGoogleService _googleService;
    private readonly IMicrosoftService _microsoftService;
    private readonly GoogleOAuth _googleOAuth;
    private readonly MicrosoftOAuth _microsoftOAuth;
    private readonly AppUrls _appUrls;

    private const string _oAuthRedirectUrlTemplate = "api/Account/oauth/{0}/callback";
    private const string _googleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth?";
    private const string _googleAccessTokenUrl = "https://oauth2.googleapis.com/token";
    private const string _microsoftAuthUrl =
        "https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize?";
    private const string _microsoftAccessTokenUrl =
        "https://login.microsoftonline.com/organizations/oauth2/v2.0/token";

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="logger">The logger used for logging information and errors.</param>
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
        ILogger<AccountController> logger,
        IUserService userService,
        IPasswordHasher passwordHasher,
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
        _logger = logger;
        _userService = userService;
        _passwordHasher = passwordHasher;
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

        user.HashPassword = _passwordHasher.HashPassword(model.NewPassword);
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
        return SignInWithOAuthProvider(
            _googleOAuth.ClientId,
            _googleOAuth.ClientSecret,
            _googleAuthUrl,
            "google",
            scope: "openid email profile",
            extraParams: new Dictionary<string, string>
            {
                { "access_type", "offline" },
                { "prompt", "consent" }
            }
        );
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
        return await GetOAuthAccessToken(
            code,
            _googleOAuth.ClientId,
            _googleOAuth.ClientSecret,
            _googleAccessTokenUrl,
            "google",
            _googleService.GetGoogleUserDetails,
            _userService.GenerateTokenUsingSSOAsync
        );
    }

    /// <summary>
    /// Sign in with Microsoft
    /// </summary>
    /// <returns>The Microsoft login interface</returns>
    [HttpGet("signin-with-microsoft")]
    [AllowAnonymous]
    public IActionResult SignInWithMicrosoft()
    {
        return SignInWithOAuthProvider(
            _microsoftOAuth.ClientId,
            _microsoftOAuth.ClientSecret,
            _microsoftAuthUrl,
            "entraId",
            scope: "openid email profile User.Read offline_access"
        );
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
        return await GetOAuthAccessToken(
            code,
            _microsoftOAuth.ClientId,
            _microsoftOAuth.ClientSecret,
            _microsoftAccessTokenUrl,
            "entraId",
            _microsoftService.GetMicrosoftUserDetails,
            _userService.GenerateTokenUsingSSOAsync
        );
    }

    /// <summary>
    /// Common method to handle OAuth sign-in.
    /// </summary>
    /// <param name="clientId">The OAuth Client ID.</param>
    /// <param name="clientSecret">The OAuth Client Secret.</param>
    /// <param name="authUrl">The authorization URL of the OAuth provider.</param>
    /// <param name="providerName">The provider name</param>
    /// <param name="scope">The scope of the OAuth request.</param>
    /// <param name="extraParams">Additional parameters to include in the OAuth request, if any.</param>
    /// <returns>The OAuth sign-in interface or an error page if configuration is missing.</returns>
    private IActionResult SignInWithOAuthProvider(
        string clientId,
        string clientSecret,
        string authUrl,
        string providerName,
        string scope,
        Dictionary<string, string> extraParams = null
    )
    {
        var redirectUrl =
            $"{_appUrls.App}/{string.Format(_oAuthRedirectUrlTemplate, providerName)}";
        var validationResult = ValidateClientConfiguration(clientId, clientSecret);
        if (validationResult != null)
        {
            return validationResult;
        }

        var nonce = CommonHelper.GenerateNonce();
        var url =
            $"{authUrl}client_id={clientId}&response_type=code&redirect_uri={UrlEncoder.Default.Encode(redirectUrl)}&response_mode=query&scope={scope}&nonce={nonce}";

        if (extraParams != null)
        {
            foreach (var param in extraParams)
            {
                url += $"&{param.Key}={param.Value}";
            }
        }

        return RedirectToFrontend(url);
    }

    /// <summary>
    /// Common method to handle OAuth access token retrieval.
    /// </summary>
    /// <param name="code">The authorization code returned by the OAuth provider.</param>
    /// <param name="clientId">The OAuth Client ID.</param>
    /// <param name="clientSecret">The OAuth Client Secret.</param>
    /// <param name="tokenUrl">The token URL of the OAuth provider.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="getUserDetails">A function to retrieve user details using the access token.</param>
    /// <param name="generateToken">A function to generate an authentication token using the retrieved user details.</param>
    /// <returns>A task representing the asynchronous operation, which returns an <see cref="IActionResult"/>.</returns>
    private async Task<IActionResult> GetOAuthAccessToken(
        string code,
        string clientId,
        string clientSecret,
        string tokenUrl,
        string providerName,
        Func<string, Task<OAuthUserResponseModel>> getUserDetails,
        Func<OAuthUserResponseModel, Task<AuthenticationModel>> generateToken
    )
    {
        var redirectUrl =
            $"{_appUrls.App}/{string.Format(_oAuthRedirectUrlTemplate, providerName)}";

        var validationResult = ValidateClientConfiguration(clientId, clientSecret);
        if (validationResult != null)
        {
            return validationResult;
        }

        var dicData = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", redirectUrl }
        };

        try
        {
            using var client = new HttpClient();
            using var content = new FormUrlEncodedContent(dicData);
            var response = await client.PostAsync(tokenUrl, content);
            var json = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

            if (tokenResponse.IsSuccess)
            {
                var user = await getUserDetails(tokenResponse.AccessToken);
                var authenticationModel = await generateToken(user);

                return RedirectToFrontend(
                    $"{_appUrls.App}/redirect/signIn?userId={authenticationModel.UserId}&token={authenticationModel.Token}&refresh={authenticationModel.RefreshToken}"
                );
            }
            else
            {
                _logger.LogError(
                    "OAuth token retrieval failed. Error: {Error}, Description: {Description}",
                    tokenResponse.Error,
                    tokenResponse.ErrorDescription
                );
                return RedirectToErrorPage(
                    _appUrls.App,
                    tokenResponse.Error,
                    tokenResponse.ErrorDescription
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the access token.");
            return RedirectToErrorPage(
                _appUrls.App,
                _localizer.GetString("TokenRetrievalError"),
                ex.Message
            );
        }
    }

    /// <summary>
    /// Validates the OAuth client configuration.
    /// </summary>
    /// <param name="clientId">The OAuth Client ID.</param>
    /// <param name="clientSecret">The OAuth Client Secret.</param>
    /// <returns>An <see cref="IActionResult"/> that redirects to an error page if validation fails, or <c>null</c> if validation passes.</returns>
    private IActionResult ValidateClientConfiguration(string clientId, string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            _logger.LogError("The client id or client secret is missing");
            return RedirectToErrorPage(
                _appUrls.App,
                _localizer.GetString("MissingClientIdOrSecret"),
                "The Client ID and Client Secret must be configured properly."
            );
        }

        return null;
    }
}
