using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProductAPI.Models;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration config,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _emailSender = emailSender;
        }

        // ✅ Register with email confirmation (fixed encoding)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match." });

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Default role assign
            await _userManager.AddToRoleAsync(user, "Student");

            // Generate confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // IMPORTANT: encode with Base64UrlEncode
            var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontUrl = _config["FrontendUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var confirmLink = $"{frontUrl}/api/Auth/confirmemail?userId={user.Id}&token={tokenEncoded}";

            // ✅ Send only via email (not in API response)
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking <a href=\"{confirmLink}\">here</a>.");

            return Ok(new { message = "User created. Please check your email to confirm your account." });
        }

        // ✅ Confirm Email (fixed decoding)
        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest("UserId and Token are required");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("Invalid user");

            // Decode token back
            var decodedBytes = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedBytes);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return Ok("Email confirmed successfully!");
            else
                return BadRequest("Email confirmation failed");
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid login attempt." });

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new { message = "Email not confirmed." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid login attempt." });

            var roles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            foreach (var r in roles) authClaims.Add(new Claim(ClaimTypes.Role, r));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddHours(4),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }

        // ✅ Forgot Password
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return Ok();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontUrl = _config["FrontendUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var resetLink = $"{frontUrl}/reset-password?userId={user.Id}&token={tokenEncoded}";

            await _emailSender.SendEmailAsync(user.Email, "Reset your password",
                $"Reset your password by clicking <a href=\"{resetLink}\">here</a>.");

            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        // ✅ Reset Password
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("Invalid request.");

            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, decoded, model.Password);

            if (result.Succeeded) return Ok(new { message = "Password reset successful." });

            return BadRequest(result.Errors);
        }
    }

    // ✅ DTOs
    public class RegisterDto { public string Email { get; set; } = null!; public string Password { get; set; } = null!; public string ConfirmPassword { get; set; } = null!; }
    public class LoginDto { public string Email { get; set; } = null!; public string Password { get; set; } = null!; }
    public class ForgotPasswordDto { public string Email { get; set; } = null!; }
    public class ResetPasswordDto { public string Email { get; set; } = null!; public string Token { get; set; } = null!; public string Password { get; set; } = null!; }
}
