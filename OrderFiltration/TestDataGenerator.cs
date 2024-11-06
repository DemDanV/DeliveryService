using System.Globalization;

static class TestDataGenerator
{
    public static void Generate()
    {
        string filePath = "orders.csv";
        int numberOfOrders = 20000;  // Количество тестовых заказов для генерации

        Random random = new Random();
        DateTime startDateTime = DateTime.Now.AddHours(-2);  // Начальная точка времени для генерации заказов

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Заголовок CSV-файла
            writer.WriteLine("OrderId;Weight;District;DeliveryTime");

            for (int i = 1; i <= numberOfOrders; i++)
            {
                // Генерация случайных данных для заказа
                string orderId = $"{i:000}";
                double weight = i == 1? -5 : Math.Round(random.NextDouble() * 10 + 1, 2);  // Вес от 1 до 10 кг
                string district = $"District{random.Next(1, 4)}";  // Случайный район, например, District1, District2
                DateTime deliveryTime = startDateTime.AddMinutes(random.Next(0, 240));  // Время доставки в пределах 4 часов от начальной точки

                // Запись строки с данными в файл
                writer.WriteLine($"{orderId};{weight};{district};{deliveryTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}");
            }
        }

        Console.WriteLine($"Test data generated successfully in file: {filePath}");
    }
}
