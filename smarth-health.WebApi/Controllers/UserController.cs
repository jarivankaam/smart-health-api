using Microsoft.AspNetCore.Mvc;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;

namespace smarth_health.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        // Getting a specific user by identityUserId

        [HttpGet("{identityUserId:guid}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromBody] Guid identityUserId)
        {
            var user = await _userRepository.ReadAsync(identityUserId);

            // If user is not found, return 404
            if (user == null) 
                return NotFound();
            return Ok(user);
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
        [HttpDelete("{userId:guid}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _userRepository.DeleteAsync(userId);
            return NoContent();
        }
    }
}
