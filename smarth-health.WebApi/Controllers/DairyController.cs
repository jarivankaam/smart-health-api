using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;

namespace smarth_health.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DairyController : ControllerBase
    {
        private readonly IDairyRepository _dairyRepository;

        public DairyController(IDairyRepository dairyRepository)
        {
            _dairyRepository = dairyRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetAllDairies()
        {
            var dairies = await _dairyRepository.GetAllAsync();
            return Ok(dairies);
        }

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDairyByUserId(Guid userId)
        {
            var dairy = await _dairyRepository.GetByUserIdAsync(userId);
            if (dairy == null)
                return NotFound();

            return Ok(dairy);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreateDairy([FromBody] Dairy dairy)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dairy.Id = Guid.NewGuid();
            await _dairyRepository.CreateAsync(dairy);
            return CreatedAtAction(nameof(GetDairyByUserId), new { userId = dairy.UserId }, dairy);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> UpdateDairy(Guid id, [FromBody] Dairy dairy)
        {
            if (id != dairy.Id)
                return BadRequest();

            var existingDairy = await _dairyRepository.GetByDairyIdAsync(id);
            if (existingDairy == null)
                return NotFound();

            await _dairyRepository.UpdateAsync(dairy);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> DeleteDairy(Guid id)
        {
            var existingDairy = await _dairyRepository.GetByDairyIdAsync(id);
            if (existingDairy == null)
                return NotFound();

            await _dairyRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}