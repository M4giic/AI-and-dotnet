using CommunicationPattern.Models;

namespace CommunicationPattern.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(Guid id, OrderStatus status);
    Task<bool> DeleteOrderAsync(Guid id);
}