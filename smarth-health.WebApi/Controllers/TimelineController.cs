using Microsoft.AspNetCore.Mvc;
using smarth_health.WebApi.Models;
using smarth_health.WebApi.Repositories;

namespace smarth_health.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimelineController : ControllerBase
{
    private readonly ITimelineRepository _timelineRepository;

    public TimelineController(ITimelineRepository timelineRepository)
    {
        _timelineRepository = timelineRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTimeline([FromBody] Timeline timeline)
    {
        timeline.ID = Guid.NewGuid();
        await _timelineRepository.InsertAsync(timeline);
        return CreatedAtAction(nameof(GetTimeline), new { id = timeline.ID }, timeline);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTimeline(Guid id)
    {
        var timeline = await _timelineRepository.ReadAsync(id);
        if (timeline == null) return NotFound();
        return Ok(timeline);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTimeline(Guid id, [FromBody] Timeline timeline)
    {
        if (id != timeline.ID) return BadRequest();
        await _timelineRepository.UpdateAsync(timeline);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTimeline(Guid id)
    {
        await _timelineRepository.DeleteAsync(id);
        return NoContent();
    }
}
