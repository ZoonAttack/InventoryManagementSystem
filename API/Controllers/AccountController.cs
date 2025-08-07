using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductsManagement.Context;
using ProductsManagement.Data;
using ProductsManagement.Data.Utility;
using ProductsManagement.DTOs;
using Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly JWTSettings _jwtSettings;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager,EmailService emailService, IOptions<JWTSettings> JWTOptions, ILogger<AccountController> Logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _jwtSettings = JWTOptions.Value;
            _logger = Logger;

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "user";
                    //Console.WriteLine(role);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, role)
                    }),
                        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                        Issuer = _jwtSettings.Issuer,
                        Audience = _jwtSettings.Audience,
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                            SecurityAlgorithms.HmacSha256)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return Ok(new { token = tokenHandler.WriteToken(token), role });
                }

                return Unauthorized(new { errors = new[] { "Invalid credentials" } });
            }

            _logger.LogWarning("Model state is invalid during login: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest("Invalid model");
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto registerDto)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };
                var result = _userManager.CreateAsync(user, registerDto.Password).Result;
                if (result.Succeeded)
                {
                    //Add user to "User" role
                    _userManager.AddToRoleAsync(user, "user").Wait();
                    return Ok(new { message = "User registered successfully" });
                }
                else
                {
                    _logger.LogError("User registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest("Invalid data");
                }
            }
            _logger.LogWarning("Model state is invalid during registration: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest("Invalid model state");
        }


        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Sign out the user
            _signInManager.SignOutAsync().Wait();
            return Ok(new { message = "User logged out successfully" });
        }


        [HttpPost("forgot-password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { errors = new[] { "Email is required" } });
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { errors = new[] { "User not found" } });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { errors = new[] { "Failed to generate password reset token" } });
            }
            string baseURL = Environment.GetEnvironmentVariable("DOMAIN");

            _emailService.EmailSend(new SendEmailDto
            {
                To = email,
                Subject = "Password Reset",
                Content = $"Please use the following token to reset your password: {baseURL}reset-password?token={token}"
            }).Wait();
            return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Token) || string.IsNullOrEmpty(dto.NewPassword))
            {
                return BadRequest(new { errors = new[] { "Invalid input" } });
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return NotFound(new { errors = new[] { "User not found" } });
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("Password reset failed for user {Email}: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest("Password reset failed");
            }

            return Ok(new { message = "Password has been reset successfully" });
        }

    }
}
