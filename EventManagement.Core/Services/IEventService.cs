using EventManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Core.Services;

public interface IEventService
{
    Task<List<Event>> GetAllEventsAsync();
    Task<Event> GetEventByIdAsync(int id);
    Task<List<Event>> GetEventsByTypeAsync(EventType type);
    Task<List<Event>> GetSubEventsAsync(int parentId);
    Task<int> CreateEventAsync(Event eventItem);
    Task UpdateEventAsync(Event eventItem);
    Task DeleteEventAsync(int id);
}