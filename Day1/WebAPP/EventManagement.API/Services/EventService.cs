using EventManagement.API.Repositories;
using EventManagement.Core.Entities;
using EventManagement.Core.Services;

namespace EventManagement.API.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<List<Event>> GetAllEventsAsync()
    {
        return await _eventRepository.GetAllAsync();
    }

    public async Task<Event> GetEventByIdAsync(int id)
    {
        return await _eventRepository.GetByIdAsync(id);
    }

    public async Task<List<Event>> GetEventsByTypeAsync(EventType type)
    {
        return await _eventRepository.GetEventsByTypeAsync(type);
    }

    public async Task<List<Event>> GetSubEventsAsync(int parentId)
    {
        return await _eventRepository.GetSubEventsAsync(parentId);
    }

    public async Task<int> CreateEventAsync(Event eventItem)
    {
        ValidateEvent(eventItem);
            
        if (eventItem.Status == 0)
        {
            eventItem.Status = EventStatus.Draft;
        }

        if (eventItem.Type == EventType.SubEvent && !eventItem.ParentEventId.HasValue)
        {
            throw new ArgumentException("Sub-events must have a parent event.");
        }

        return await _eventRepository.AddAsync(eventItem);
    }

    public async Task UpdateEventAsync(Event eventItem)
    {
        ValidateEvent(eventItem);

        // Check for date range conflicts for sub-events
        if (eventItem.Type == EventType.SubEvent && eventItem.ParentEventId.HasValue)
        {
            var parentEvent = await GetEventByIdAsync(eventItem.ParentEventId.Value);
            if (parentEvent != null)
            {
                if (eventItem.StartDate < parentEvent.StartDate || eventItem.EndDate > parentEvent.EndDate)
                {
                    throw new ArgumentException("Sub-event dates must be within the parent event date range.");
                }
            }
        }

        await _eventRepository.UpdateAsync(eventItem);
    }

    public async Task DeleteEventAsync(int id)
    {
        var eventToDelete = await GetEventByIdAsync(id);
        if (eventToDelete == null)
        {
            throw new ArgumentException($"Event with ID {id} not found.");
        }

        // Check if it's a series event and has sub-events
        if (eventToDelete.Type == EventType.Series)
        {
            var subEvents = await GetSubEventsAsync(id);
            if (subEvents.Any())
            {
                // Delete all sub-events first
                foreach (var subEvent in subEvents)
                {
                    await _eventRepository.DeleteAsync(subEvent.Id);
                }
            }
        }

        await _eventRepository.DeleteAsync(id);
    }

    private void ValidateEvent(Event eventItem)
    {
        if (string.IsNullOrWhiteSpace(eventItem.Title))
        {
            throw new ArgumentException("Event title is required.");
        }

        if (string.IsNullOrWhiteSpace(eventItem.Description))
        {
            throw new ArgumentException("Event description is required.");
        }

        if (eventItem.StartDate > eventItem.EndDate)
        {
            throw new ArgumentException("Event end date must be after start date.");
        }

        if (eventItem.Capacity <= 0)
        {
            throw new ArgumentException("Event capacity must be greater than zero.");
        }
    }
}