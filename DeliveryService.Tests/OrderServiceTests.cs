using Moq;
using OrderFiltration;

public class OrderServiceTests
{
    [Fact]
    public void ValidateOrders_ShouldReturnOnlyValidOrders()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var orderService = new OrderService(loggerMock.Object);

        var orders = new List<Order>
        {
            new Order { Id = 1, Weight = 10, District = "A", DeliveryTime = DateTime.Now },
            new Order { Id = 2, Weight = -5, District = "A", DeliveryTime = DateTime.Now } // Invalid weight
        };

        // Act
        var validOrders = orderService.ValidateOrders(orders);

        // Assert
        Assert.Single(validOrders);
        Assert.Equal(1, validOrders.First().Id);
    }

    [Fact]
    public void FilterOrders_ShouldReturnOrdersWithinTimeFrame()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var orderService = new OrderService(loggerMock.Object);

        var firstDeliveryTime = DateTime.Now;
        var orders = new List<Order>
        {
            new Order { Id = 1, District = "A", DeliveryTime = firstDeliveryTime.AddMinutes(15) },
            new Order { Id = 2, District = "A", DeliveryTime = firstDeliveryTime.AddMinutes(45) }, // Outside the timeframe
            new Order { Id = 3, District = "B", DeliveryTime = firstDeliveryTime.AddMinutes(20) }  // Different district
        };

        // Act
        var filteredOrders = orderService.FilterOrders(orders, "A", firstDeliveryTime);

        // Assert
        Assert.Single(filteredOrders);
        Assert.Equal(1, filteredOrders.First().Id);
    }
}
