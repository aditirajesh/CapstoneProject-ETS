using System.Security.Claims;
using ExpenseTrackingSystem.Interfaces;
using ExpenseTrackingSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingSystem.Controllers
{
    [Route("auth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly ITokenService _tokenService;

        public OAuthController(IRepository<string, User> userRepo, ITokenService tokenService)
        {
            _userRepository = userRepo;
            _tokenService = tokenService;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "OAuth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest("OAuth login failed");

            var email = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return BadRequest("Email not found in claims");

            var user = await _userRepository.GetByID(email);
            if (user == null)
            {
                user = await _userRepository.Add(new User
                {
                    Username = email,
                    Role = "User"
                });
            }
            var pictureUrl = authenticateResult.Principal?.FindFirst("urn:google:picture")?.Value;

            int expiryMinutes = 2;
            var jwtToken = await _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect($"http://localhost:4200/oauth-callback?username={user.Username}&accessToken={jwtToken}&refreshToken={refreshToken}&picture={Uri.EscapeDataString(pictureUrl)}&expiry={expiryMinutes}");
            // return Ok(new
            // {
            //     jwtToken = jwtToken
            // });
        }
    }
}
