using System;

namespace SmartOrderProcessor
{
    /// <summary>
    /// Аргументы события успешной обработки заказа.
    /// </summary>
    public sealed class OrderProcessedEventArgs : EventArgs
    {
        #region Свойства
        public int OrderId { get; }
        public decimal Total { get; }
        public int ProcessedItemsCount { get; }
        public DateTimeOffset ProcessedAt { get; }
        #endregion

        #region Конструкторы
        public OrderProcessedEventArgs(int orderId, decimal total, int processedItemsCount, DateTimeOffset processedAt)
        {
            OrderId = orderId;
            Total = total;
            ProcessedItemsCount = processedItemsCount;
            ProcessedAt = processedAt;
        }
        #endregion
    }
}