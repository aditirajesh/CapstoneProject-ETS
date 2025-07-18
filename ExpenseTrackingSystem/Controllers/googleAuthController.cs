using System.Security.Claims;
using ExpenseTrackingSystem.Interfaces;
using ExpenseTrackingSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExpenseTrackingSystem.Controllers
{
    [Route("auth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<OAuthController> _logger;

        public OAuthController(IRepository<string, User> userRepo, ITokenService tokenService, ILogger<OAuthController> logger)
        {
            _userRepository = userRepo;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "OAuth");
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                AllowRefresh = true
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                _logger.LogWarning("Google OAuth failed or user principal is null.");
                return BadRequest("OAuth login failed");
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var pictureUrl = authenticateResult.Principal.FindFirst("urn:google:picture")?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Google login missing email claim.");
                return BadRequest("Email not found in claims");
            }

            _logger.LogInformation("Google login successful for email: {Email}", email);

            // Register user if not exists
            var user = await _userRepository.GetByID(email);
            if (user == null)
            {
                _logger.LogInformation("Registering new user from Google OAuth: {Email}", email);
                user = await _userRepository.Add(new User
                {
                    Username = email,
                    Role = "User"
                });
            }

            var accessToken = await _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var expiryMinutes = 2; // or get from config

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // optional cleanup

            // Redirect with token info
            var redirectUrl = $"http://localhost:4200/oauth-callback" +
                              $"?username={Uri.EscapeDataString(user.Username)}" +
                              $"&accessToken={Uri.EscapeDataString(accessToken)}" +
                              $"&refreshToken={Uri.EscapeDataString(refreshToken)}" +
                              $"&picture={Uri.EscapeDataString(pictureUrl ?? "")}" +
                              $"&expiry={expiryMinutes}";

            return Redirect(redirectUrl);
        }
    }
}
