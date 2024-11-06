using OrderFiltration;

public class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger logger)
    {
        _logger = logger;
    }

    // Метод для фильтрации заказов
    public IEnumerable<Order> FilterOrders(IEnumerable<Order> orders, string district, DateTime firstDeliveryTime)
    {
        _logger.Log("Starting order filtering process.");

        // Определяем временное окно для фильтрации
        DateTime timeWindowEnd = firstDeliveryTime.AddMinutes(30);

        // Применяем фильтры по району и времени доставки
        var filteredOrders = orders
            .Where(order => order.District == district
                            && order.DeliveryTime >= firstDeliveryTime
                            && order.DeliveryTime <= timeWindowEnd);

        _logger.Log($"Found {filteredOrders.Count()} orders for district '{district}' within the time window.");

        return filteredOrders;
    }

    // Метод для валидации заказов
    public IEnumerable<Order> ValidateOrders(IEnumerable<Order> orders)
    {
        _logger.Log("Starting order validation process.");

        var validOrders = new List<Order>();

        foreach (var order in orders)
        {
            if (IsValidOrder(order))
            {
                validOrders.Add(order);
            }
            else
            {
                _logger.Log($"Invalid order data found: {order.Id}");
            }
        }

        _logger.Log($"Validation complete. {validOrders.Count} valid orders found.");
        return validOrders;
    }

    // Пример метода валидации одного заказа
    private bool IsValidOrder(Order order)
    {
        // Проверяем, что вес положительный и время доставки корректно
        return order.Weight > 0 && order.DeliveryTime > DateTime.MinValue && !string.IsNullOrEmpty(order.District);
    }
}
