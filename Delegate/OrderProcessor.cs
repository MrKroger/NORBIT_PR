using System;
using System.Collections.Generic;

namespace SmartOrderProcessor
{
    /// <summary>
    /// Класс для обработки заказов с использованием делегатов и событий.
    /// </summary>
    public sealed class OrderProcessor
    {
        #region События
        /// <summary>
        /// Событие, возникающее после успешной обработки заказа.
        /// </summary>
        public event EventHandler<OrderProcessedEventArgs>? OrderProcessed;
        #endregion

        #region Публичные методы
        /// <summary>
        /// Обрабатывает заказ, применяя фильтрацию, трансформацию цен и расчет стоимости.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа (должен быть > 0).</param>
        /// <param name="items">Коллекция товаров.</param>
        /// <param name="shouldProcess">Предикат для фильтрации товаров.</param>
        /// <param name="transformPrice">Функция для преобразования цены.</param>
        /// <param name="calculateItemTotal">Делегат для расчета итоговой стоимости позиции.</param>
        /// <param name="log">Опциональный делегат для логирования.</param>
        /// <returns>Итоговая сумма заказа.</returns>
        public decimal Process(
            int orderId,
            IEnumerable<OrderItem> items,
            Predicate<OrderItem> shouldProcess,
            Func<decimal, decimal> transformPrice,
            ItemTotalCalculator calculateItemTotal,
            Action<string>? log = null)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(shouldProcess);
            ArgumentNullException.ThrowIfNull(transformPrice);
            ArgumentNullException.ThrowIfNull(calculateItemTotal);
            
            if (orderId <= 0)
                throw new ArgumentOutOfRangeException(nameof(orderId), "ID заказа должен быть положительным.");

            log?.Invoke($"[LOG] Начало обработки заказа #{orderId}.");

            decimal total = 0;
            int processedItemsCount = 0;

            foreach (var item in items)
            {
                if (item.Price < 0)
                    throw new ArgumentOutOfRangeException(nameof(item), $"Цена товара '{item.Name}' не может быть отрицательной.");
                if (item.Quantity <= 0)
                    throw new ArgumentOutOfRangeException(nameof(item), $"Количество товара '{item.Name}' должно быть больше нуля.");

                if (!shouldProcess(item))
                {
                    log?.Invoke($"[LOG] Товар '{item.Name}' исключен из расчета.");
                    continue;
                }

                decimal transformedPrice = transformPrice(item.Price);
                log?.Invoke($"[LOG] Цена товара '{item.Name}' изменена с {item.Price} на {transformedPrice}.");

                decimal itemTotal = calculateItemTotal(item, transformedPrice);
                log?.Invoke($"[LOG] Стоимость позиции '{item.Name}': {itemTotal}.");

                total += itemTotal;
                processedItemsCount++;
            }

            log?.Invoke($"[LOG] Завершение обработки заказа #{orderId}. Итого: {total}.");

            var eventArgs = new OrderProcessedEventArgs(orderId, total, processedItemsCount, DateTimeOffset.Now);
            OnOrderProcessed(eventArgs);

            return total;
        }
        #endregion

        #region Приватные методы
        /// <summary>
        /// Вызывает событие OrderProcessed с безопасной обработкой исключений в цепочке подписчиков.
        /// </summary>
        private void OnOrderProcessed(OrderProcessedEventArgs eventArgs)
        {
            if (OrderProcessed == null) return;

            // Получаем список всех подписчиков
            foreach (var handler in OrderProcessed.GetInvocationList())
            {
                try
                {
                    ((EventHandler<OrderProcessedEventArgs>)handler).Invoke(this, eventArgs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ОШИБКА ПОДПИСЧИКА] Один из обработчиков вызвал исключение: {ex.Message}");
                }
            }
        }
        #endregion
    }
}