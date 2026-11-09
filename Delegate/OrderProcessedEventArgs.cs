using System;

namespace SmartOrderProcessor
{
    /// <summary>
    /// Аргументы события успешной обработки заказа.
    /// </summary>
    public sealed class OrderProcessedEventArgs : EventArgs
    {
        /// <summary>
        /// Инициализирует новый экземпляр аргументов события.
        /// </summary>
        public OrderProcessedEventArgs(int orderId, decimal total, int processedItemsCount, DateTimeOffset processedAt)
        {
            OrderId = orderId;
            Total = total;
            ProcessedItemsCount = processedItemsCount;
            ProcessedAt = processedAt;
        }

        /// <summary>
        /// Идентификатор обработанного заказа.
        /// </summary>
        public int OrderId { get; }
        
        /// <summary>
        /// Итоговая сумма заказа.
        /// </summary>
        public decimal Total { get; }
        
        /// <summary>
        /// Количество успешно обработанных позиций.
        /// </summary>
        public int ProcessedItemsCount { get; }
        
        /// <summary>
        /// Дата и время завершения обработки.
        /// </summary>
        public DateTimeOffset ProcessedAt { get; }
    }
}