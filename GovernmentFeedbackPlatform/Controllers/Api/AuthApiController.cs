using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentFeedbackPlatform.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthApiController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>POST /api/auth/register – Register a new citizen account</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
                return Conflict(new { message = "An account with this email already exists." });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                State = model.State,
                Language = model.Language,
                AgeGroup = model.AgeGroup,
                Gender = model.Gender,
                Occupation = model.Occupation,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Citizen");
                return Ok(new
                {
                    message = "Citizen registration successful.",
                    userId = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    role = "Citizen"
                });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        /// <summary>POST /api/auth/login – Authenticate citizen or admin user</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or credentials." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    message = "Login successful.",
                    userId = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    roles = roles
                });
            }

            return Unauthorized(new { message = "Invalid email or credentials." });
        }
    }
}
