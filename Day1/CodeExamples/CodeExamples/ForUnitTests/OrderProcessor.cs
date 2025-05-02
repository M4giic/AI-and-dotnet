namespace CodeExamples.ForUnitTests;

public class OrderProcessor
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;

    public OrderProcessor(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IEmailService emailService)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _emailService = emailService;
    }

    public async Task<OrderResult> ProcessOrderAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.Items == null || !order.Items.Any())
            return new OrderResult { Success = false, ErrorMessage = "Order contains no items" };

        // Validate stock availability
        foreach (var item in order.Items)
        {
            var stockAvailable = await _orderRepository.CheckStockAsync(item.ProductId, item.Quantity);
            if (!stockAvailable)
                return new OrderResult { Success = false, ErrorMessage = $"Insufficient stock for product {item.ProductId}" };
        }

        // Process payment
        var paymentResult = await _paymentService.ProcessPaymentAsync(order.PaymentDetails, order.TotalAmount);
        if (!paymentResult.Success)
            return new OrderResult { Success = false, ErrorMessage = $"Payment failed: {paymentResult.ErrorMessage}" };

        // Save order
        var orderId = await _orderRepository.SaveOrderAsync(order);
        
        // Send confirmation email
        await _emailService.SendOrderConfirmationAsync(order.CustomerEmail, orderId);

        return new OrderResult { Success = true, OrderId = orderId };
    }
}

public class Order
{
    public Guid Id { get; set; }
    public List<OrderItem> Items { get; set; }
    public PaymentDetails PaymentDetails { get; set; }
    public decimal TotalAmount { get; set; }
    public string CustomerEmail { get; set; }
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PaymentDetails
{
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public string Cvv { get; set; }
    public string CardholderName { get; set; }
}

public class OrderResult
{
    public bool Success { get; set; }
    public Guid OrderId { get; set; }
    public string ErrorMessage { get; set; }
}

public interface IOrderRepository
{
    Task<bool> CheckStockAsync(Guid productId, int quantity);
    Task<Guid> SaveOrderAsync(Order order);
}

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentDetails paymentDetails, decimal amount);
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string ErrorMessage { get; set; }
}

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string email, Guid orderId);
}