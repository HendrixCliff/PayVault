using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PayVault.Application.Interfaces.Services;
using PayVault.Infrastructure.Auth;
using PayVault.Application.DTOs.Auth;
using PayVault.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization; 
using PayVault.Domain.Templates;  
using System.Net;

namespace PayVault.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwt;
        private readonly IEmailService _emailService;
        private readonly IUserManagementService _userService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwt,
            IEmailService emailService,
            IUserManagementService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
            _emailService = emailService;
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(RegisterDto dto)
        {
            var userId = await _userService.RegisterAsync(dto);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest(new { message = "User creation failed" });

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") 
                          ?? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var link = $"{baseUrl}/api/auth/confirmemail?userId={user.Id}&token={encodedToken}";

            var fullName = user.FullName ?? $"{user.FirstName} {user.LastName}".Trim();

            await _emailService.SendAsync(
                user.Email!,
                "Confirm your PayVault account",
                EmailTemplates.ConfirmAccount(fullName, link)
            );

            return Ok(new
            {
                message = "User created! Check your email to confirm account.",
                email = user.Email,
                fullName
            });
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Invalid confirmation request" });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (user.EmailConfirmed)
                return Ok(new { message = "Email already confirmed. Login now!" });

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return Ok(new
                {
                    message = "Email confirmed! You can now login.",
                    email = user.Email
                });

            return BadRequest(new
            {
                message = "Confirmation failed. Token expired?",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new { message = "Invalid credentials" });

            if (!user.EmailConfirmed)
                return Unauthorized(new { message = "Confirm email first" });

            var tokens = await _jwt.GenerateTokensAsync(user);

            return Ok(new AuthResponseDto
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            });
        }

        [HttpPost("seed-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedAdmin()
        {
            var adminEmail = "admin@payvault.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Hendrixnigga0799");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    return Ok(new { message = "Admin created successfully!" });
                }

                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Admin already exists" });
        }

        [HttpPost("reset-admin-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetAdminPassword()
        {
            var adminEmail = "admin@payvault.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(adminUser);

                var result = await _userManager.ResetPasswordAsync(adminUser, token, "Hendrixnigga0799");

                if (result.Succeeded)
                    return Ok(new { message = " Admin password reset successfully!" });

                return BadRequest(result.Errors);
            }

            return NotFound("Admin user not found");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken))
                return BadRequest(new { message = "Invalid refresh token" });


            var userId = "temp-user-id";

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized(new { message = "Invalid user" });

            var tokens = await _jwt.GenerateTokensAsync(user);

            return Ok(new AuthResponseDto
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "User logged out successfully" });
        }
    }
}