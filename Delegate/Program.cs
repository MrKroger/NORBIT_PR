using System;
using System.Collections.Generic;

namespace SmartOrderProcessor
{
    /// <summary>
    /// Главный класс программы для демонстрации работы OrderProcessor.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Демонстрация работы OrderProcessor ---");

            var orderItems = new List<OrderItem>
            {
                new OrderItem("Ноутбук", 1000m, 1),
                new OrderItem("Мышь", 50m, 2),
                new OrderItem("Монитор", 300m, 1),
                new OrderItem("Сломанный кабель", 0m, 5)
            };

            var processor = new OrderProcessor();

            // 1. Замыкание (Closure): переменная discount захватывается
            decimal discount = 0.1m;
            Func<decimal, decimal> applyDiscount = price => price * (1 - discount);

            // 2. Статическая лямбда (Static Lambda): не захватывает контекст
            Predicate<OrderItem> filterValidItems = static item => item.Price > 0;

            // 3. Кастомный делегат
            ItemTotalCalculator calculateTotal = (item, transformedPrice) => transformedPrice * item.Quantity;

            // 4. Логгер (Action)
            Action<string> logger = message => Console.WriteLine(message);

            // 5. Подписка на события (Мультикаст)
            EventHandler<OrderProcessedEventArgs> emailHandler = (sender, e) => 
                Console.WriteLine($"[EMAIL] Чек для заказа #{e.OrderId} на сумму {e.Total} отправлен.");
            
            EventHandler<OrderProcessedEventArgs> smsHandler = (sender, e) => 
                Console.WriteLine($"[SMS] Уведомление о заказе #{e.OrderId} отправлено.");

            EventHandler<OrderProcessedEventArgs> brokenHandler = (sender, e) => 
                throw new Exception("Сервер уведомлений недоступен!");

            processor.OrderProcessed += emailHandler;
            processor.OrderProcessed += smsHandler;
            processor.OrderProcessed += brokenHandler;

            Console.WriteLine("\n--- Запуск процесса (с ошибкой в подписчике) ---");
            try
            {
                decimal total = processor.Process(
                    orderId: 101,
                    items: orderItems,
                    shouldProcess: filterValidItems,
                    transformPrice: applyDiscount,
                    calculateItemTotal: calculateTotal,
                    log: logger
                );

                Console.WriteLine($"\nУСПЕХ: Итоговая стоимость заказа: {total:f2} руб.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
            }

            // 6. Отписка
            Console.WriteLine("\n--- Отписываем сломанный обработчик и запускаем снова ---");
            processor.OrderProcessed -= brokenHandler;
            processor.OrderProcessed -= emailHandler;

            processor.Process(
                orderId: 102,
                items: new List<OrderItem> { new OrderItem("Книга", 20m, 1) },
                shouldProcess: filterValidItems,
                transformPrice: applyDiscount,
                calculateItemTotal: calculateTotal,
                log: null
            );

            Console.ReadLine();
        }
    }
}