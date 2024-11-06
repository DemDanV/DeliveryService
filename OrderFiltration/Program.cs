using OrderFiltration;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        // Генерация тестовых данных (опционально, можно убрать для продакшн-версии)
        TestDataGenerator.Generate();

        // Проверка наличия аргументов
        if (args.Length < 2)
        {
            Console.WriteLine("Ошибка: недостаточно аргументов. Укажите параметры: _cityDistrict=[район] _firstDeliveryDateTime=[время первого заказа] _deliveryLog=[путь к файлу логов] _deliveryOrder=[путь к файлу результатов]");
            return;
        }

        // Инициализация переменных для параметров
        string district = null;
        DateTime firstDeliveryTime = DateTime.MinValue;
        string logFilePath = "log.txt";
        string outputFilePath = "filtered_orders.csv";

        // Парсинг аргументов командной строки
        foreach (var arg in args)
        {
            var splitArg = arg.Split('=', 2); // Ограничиваем количество частей для предотвращения проблем с вложенными `=`
            if (splitArg.Length != 2)
            {
                Console.WriteLine($"Предупреждение: некорректных аргумент {arg} будет проигнорирован.");
                continue; // Пропуск некорректных аргументов
            }

            var key = splitArg[0].Trim();
            var value = splitArg[1].Trim();

            switch (key)
            {
                case "_cityDistrict":
                    district = value;
                    break;
                case "_firstDeliveryDateTime":
                    if (!DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out firstDeliveryTime))
                    {
                        Console.WriteLine("Ошибка: неверный формат времени для первого заказа (_firstDeliveryDateTime). Используйте формат: yyyy-MM-dd HH:mm:ss");
                        return;
                    }
                    break;
                case "_deliveryLog":
                    logFilePath = value;
                    EnsureFileExtension(ref logFilePath, ".txt");
                    break;
                case "_deliveryOrder":
                    outputFilePath = value;
                    EnsureFileExtension(ref outputFilePath, ".csv");
                    break;
                default:
                    Console.WriteLine($"Предупреждение: нераспознанный аргумент {key} будет проигнорирован.");
                    break;
            }
        }

        // Проверка обязательных параметров
        if (string.IsNullOrEmpty(district))
        {
            Console.WriteLine("Ошибка: параметр _cityDistrict не указан.");
            return;
        }
        if (firstDeliveryTime == DateTime.MinValue)
        {
            Console.WriteLine("Ошибка: параметр _firstDeliveryDateTime не указан или неверный.");
            return;
        }

        Console.WriteLine($"Фильтрация заказов для района '{district}' начиная с {firstDeliveryTime}");

        // Логгер для записи событий
        var logger = new Logger(logFilePath);
        logger.Log("Приложение запущено.");

        try
        {
            // Инициализация репозитория и загрузка заказов
            var orderRepository = new OrderRepository("orders.csv", logger);
            var orders = orderRepository.GetOrders();

            // Создание службы обработки заказов и выполнение валидации
            var orderService = new OrderService(logger);
            var validOrders = orderService.ValidateOrders(orders);

            // Фильтрация заказов
            var filteredOrders = orderService.FilterOrders(validOrders, district, firstDeliveryTime);

            // Сохранение результатов фильтрации
            orderRepository.SaveOrders(filteredOrders, outputFilePath);
            logger.Log("Фильтрация успешно завершена. Результаты сохранены.");
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка: {ex.Message}");
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
        finally
        {
            logger.Log("Приложение завершено.");
        }
    }

    /// <summary>
    /// Проверяет и добавляет расширение файла, если оно отсутствует.
    /// </summary>
    private static void EnsureFileExtension(ref string filePath, string defaultExtension)
    {
        if (!Path.HasExtension(filePath))
        {
            filePath += defaultExtension;
        }
    }
}
