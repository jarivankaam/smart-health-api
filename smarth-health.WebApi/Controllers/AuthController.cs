using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;

namespace smarth_health.WebApi.Controllers
{
    [ApiController]
    [Route("custom/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;

        public AuthController(UserManager<IdentityUser> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the IdentityUser
            var identityUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Creating a new User (non IdentityUser)
            var appUser = new User
            {
                ID = Guid.NewGuid(),
                IdentityUserID = Guid.Parse(identityUser.Id),
                DisplayName = model.Email,
                ProfilePhotoPath = null
            };

            await _userRepository.InsertAsync(appUser);

            return Ok("User registered successfully.");
        }
    }
}
