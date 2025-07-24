using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductsManagement.Context;
using ProductsManagement.Data;
using ProductsManagement.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace ProductsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [HttpPost("login")]
        //public async Task<IActionResult> Login(LoginUserDto loginDto)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        User user = await _userManager.FindByEmailAsync(loginDto.Email);

        //        if(user is not null)
        //        {
        //            bool exist = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        //            if(exist)
        //            {
        //            }
        //            else
        //            {

        //            }
        //        }
        //        else
        //        {
        //            return NotFound(new { errors = new[] { "User not found" } });
        //        }
        //    }
        //    return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        //}

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
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }
            } return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
    }
}
