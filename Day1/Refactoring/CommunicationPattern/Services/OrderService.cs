using CommunicationPattern.Models;
using CommunicationPattern.Repositories;

namespace CommunicationPattern.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllOrdersAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.GetOrderByIdAsync(id);
    }

    public async Task<Order> CreateOrderAsync(OrderCreateRequest request)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            TotalPrice = request.UnitPrice * request.Quantity
        };

        return await _orderRepository.CreateOrderAsync(order);
    }

    public async Task<Order?> UpdateOrderStatusAsync(Guid id, OrderUpdateRequest request)
    {
        return await _orderRepository.UpdateOrderAsync(id, request.Status);
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        return await _orderRepository.DeleteOrderAsync(id);
    }
}