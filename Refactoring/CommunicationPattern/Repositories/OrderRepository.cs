using CommunicationPattern.Data;
using CommunicationPattern.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunicationPattern.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderContext _context;

    public OrderRepository(OrderContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.Id = Guid.NewGuid();
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatus.Pending;
            
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
            
        return order;
    }

    public async Task<Order?> UpdateOrderAsync(Guid id, OrderStatus status)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            return null;

        order.Status = status;
        await _context.SaveChangesAsync();
            
        return order;
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
            
        return true;
    }
}