using Moq;
using OrderFiltration;

public class OrderRepositoryTests
{
    [Fact]
    public void GetOrders_ShouldReturnOrdersFromFile()
    {
        // Arrange
        var filePath = "test_orders.csv";
        File.WriteAllText(filePath, "Id;Weight;District;DeliveryTime\n1;5;A;2024-11-06 10:00:00\n2;10;B;2024-11-06 10:30:00");

        var loggerMock = new Mock<ILogger>();
        var repository = new OrderRepository(filePath, loggerMock.Object);

        // Act
        var orders = repository.GetOrders();

        // Assert
        Assert.Equal(2, orders.Count);
        Assert.Equal(1, orders.First().Id);
        Assert.Equal("A", orders.First().District);

        // Cleanup
        File.Delete(filePath);
    }
}
