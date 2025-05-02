using EventManagement.Core.Entities;

namespace EventManagement.API.Repositories;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync();
    Task<Event> GetByIdAsync(int id);
    Task<List<Event>> GetEventsByTypeAsync(EventType type);
    Task<List<Event>> GetSubEventsAsync(int parentId);
    Task<int> AddAsync(Event eventItem);
    Task UpdateAsync(Event eventItem);
    Task DeleteAsync(int id);
}