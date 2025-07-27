using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductsManagement.Data.Utility;
using ProductsManagement.Data;
using ProductsManagement.DTOs;

namespace ProductsManagement.Controllers
{
       
        [Route("api/admin/auth")]
        [ApiController]
        public class AdminAuthController : ControllerBase
        {
            private readonly JWTSettings _jwtSettings;
            private readonly UserManager<User> _userManager;

            public AdminAuthController(UserManager<User> userManager, IOptions<JWTSettings> jwtOptions)
            {
                _userManager = userManager;
                _jwtSettings = jwtOptions.Value;
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login(LoginUserDto loginDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    return Unauthorized(new { errors = new[] { "Invalid credentials" } });
                }

                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    return Unauthorized(new { errors = new[] { "Access denied. User is not an Admin." } });
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                        SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { token = tokenHandler.WriteToken(token), role = "Admin" });
           
              
        
        }
        }
    }

