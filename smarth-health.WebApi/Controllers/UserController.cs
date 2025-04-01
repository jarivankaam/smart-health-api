using Microsoft.AspNetCore.Mvc;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;
using smarth_health.WebApi.Services;

namespace smarth_health.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        public UserController(IUserRepository userRepository, IIdentityService identityService)
        {
            _userRepository = userRepository;
            _identityService = identityService;
        }

        // POST

        // Creating a new user
        [HttpPost(Name = "CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            user.ID = Guid.NewGuid();
            await _userRepository.InsertAsync(user);
            return CreatedAtAction(nameof(GetUser), new { identityUserId = user.IdentityUserID }, user);
        }

        // GET / READ

        [HttpGet("CurrentUser")]
        public async Task<ActionResult<Guid>> GetIdentityIdByUser()
        {
            try
            {
                var identityUserId = await _identityService.GetCurrentUserIdAsync(User);

                var user = await _userRepository.ReadAsync(Guid.Parse(identityUserId));

                // If user is not found, return 404
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(Guid.Parse(user.IdentityUserID));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }
        }

        // Getting a specific user by identityUserId

        [HttpGet("{identityUserId:guid}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser([FromBody] Guid identityUserId)
        {
            var user = await _userRepository.ReadAsync(identityUserId);

            // If user is not found, return 404
            if (user == null)
                return NotFound();

            // Mapping
            var mappedUser = new User()
            {
                ID = user.ID,
                IdentityUserID = Guid.Parse(user.IdentityUserID),
                DisplayName = user.DisplayName,
                ProfilePhotoPath = user.ProfilePhotoPath
            };

            return Ok(mappedUser);
        }

        // UPDATE

        // Updating user by userId
        [HttpPut("{userId:guid}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] User user)
        {
            // Check if userId is the same as user.ID
            if (userId != user.ID) 
                return BadRequest();
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        // DELETE

        // Deleting user by userId
        [HttpDelete("{identityUserIderId:guid}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(Guid identityUserIderId)
        {
            await _userRepository.DeleteAsync(identityUserIderId);
            return NoContent();
        }
    }
}
