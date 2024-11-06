using OrderFiltration;
using System.Globalization;

public class OrderRepository
{
    private readonly string _filePath;
    private ILogger _logger;

    public OrderRepository(string filePath, ILogger logger)
    {
        _filePath = filePath;
        _logger = logger;
    }

    // Метод для чтения заказов из файла
    public List<Order> GetOrders()
    {
        List<Order> orders = new List<Order>();

        try
        {
            using (StreamReader reader = new StreamReader(_filePath))
            {
                // Пропустить первую строку с заголовками столбцов
                reader.ReadLine();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var order = ParseOrder(line);
                    if (order != null)
                    {
                        orders.Add(order);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Error reading orders: {ex.Message}");
            Console.WriteLine($"Error reading orders: {ex.Message}");
        }

        return orders;
    }

    // Метод для записи отфильтрованных заказов в файл
    public void SaveOrders(IEnumerable<Order> orders, string outputPath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                foreach (var order in orders)
                {
                    writer.WriteLine($"{order.Id};{order.Weight};{order.District};{order.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Error saving orders: {ex.Message}");
            Console.WriteLine($"Error saving orders: {ex.Message}");
        }
    }

    // Метод для преобразования строки из файла в объект Order
    private Order ParseOrder(string line)
    {
        var parts = line.Split(';');

        if (parts.Length < 4)
        {
            _logger.Log($"Invalid order data: {line}");
            Console.WriteLine($"Invalid order data: {line}");
            return null;
        }

        try
        {
            return new Order
            {
                Id = int.Parse(parts[0]),
                Weight = double.Parse(parts[1]),
                District = parts[2],
                DeliveryTime = DateTime.Parse(parts[3], CultureInfo.InvariantCulture, DateTimeStyles.None)
            };
        }
        catch (Exception ex)
        {
            _logger.Log($"Error parsing order: {ex.Message}");
            Console.WriteLine($"Error parsing order: {ex.Message}");
            return null;
        }
    }
}
