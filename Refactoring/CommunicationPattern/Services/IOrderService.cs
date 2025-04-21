using CommunicationPattern.Models;

namespace CommunicationPattern.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(OrderCreateRequest request);
    Task<Order?> UpdateOrderStatusAsync(Guid id, OrderUpdateRequest request);
    Task<bool> DeleteOrderAsync(Guid id);
}