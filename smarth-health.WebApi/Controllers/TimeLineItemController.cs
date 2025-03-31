using Microsoft.AspNetCore.Mvc;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;

namespace smarth_health.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeLineItemsController : ControllerBase
    {
        private readonly ITimeLineItemRepository _timeLineItemRepository;

        public TimeLineItemsController(ITimeLineItemRepository timeLineItemRepository)
        {
            _timeLineItemRepository = timeLineItemRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var items = await _timeLineItemRepository.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _timeLineItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(TimeLineItem item)
        {
            item.ID = Guid.NewGuid();
            await _timeLineItemRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetById), new { id = item.ID }, item);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, TimeLineItem item)
        {
            if (id != item.ID)
                return BadRequest();

            var existingItem = await _timeLineItemRepository.GetByIdAsync(id);
            if (existingItem == null)
                return NotFound();

            await _timeLineItemRepository.UpdateAsync(item);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingItem = await _timeLineItemRepository.GetByIdAsync(id);
            if (existingItem == null)
                return NotFound();

            await _timeLineItemRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
