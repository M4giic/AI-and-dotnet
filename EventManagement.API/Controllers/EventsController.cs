using EventManagement.Core.Entities;
using EventManagement.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        var events = await _eventService.GetAllEventsAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(int id)
    {
        var @event = await _eventService.GetEventByIdAsync(id);
        if (@event == null)
        {
            return NotFound();
        }
        return Ok(@event);
    }

    [HttpPost]
    public async Task<ActionResult<Event>> CreateEvent(Event @event)
    {
        var id = await _eventService.CreateEventAsync(@event);
        return CreatedAtAction(nameof(GetEvent), new { id }, @event);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, Event @event)
    {
        if (id != @event.Id)
        {
            return BadRequest();
        }

        await _eventService.UpdateEventAsync(@event);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        await _eventService.DeleteEventAsync(id);
        return NoContent();
    }
}