using EventManagement.Core.Data;
using EventManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagement.Core.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventManagementDbContext _context;

    public EventRepository(EventManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        var events = await _context.Events
            .Include(e => e.ParentEvent)
            .ToListAsync();
        return events;
    }

    public async Task<Event> GetByIdAsync(int id)
    {
        return await _context.Events
            .Include(e => e.ParentEvent)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Event>> GetEventsByTypeAsync(EventType type)
    {
        return await _context.Events
            .Where(e => e.Type == type)
            .ToListAsync();
    }

    public async Task<List<Event>> GetSubEventsAsync(int parentId)
    {
        return await _context.Events
            .Where(e => e.ParentEventId == parentId)
            .ToListAsync();
    }

    public async Task<int> AddAsync(Event eventItem)
    {
        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();
        return eventItem.Id;
    }

    public async Task UpdateAsync(Event eventItem)
    {
        _context.Entry(eventItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var eventToDelete = await _context.Events.FindAsync(id);
        if (eventToDelete != null)
        {
            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
        }
    }
}