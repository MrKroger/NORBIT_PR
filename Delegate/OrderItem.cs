namespace SmartOrderProcessor
{
    /// <summary>
    /// Товарная позиция в заказе.
    /// </summary>
    public sealed class OrderItem
    {
        #region Свойства
        /// <summary>
        /// Название товара.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Цена за единицу.
        /// </summary>
        public decimal Price { get; }
        
        /// <summary>
        /// Количество единиц товара.
        /// </summary>
        public int Quantity { get; }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Инициализирует новый экземпляр товарной позиции.
        /// </summary>
        public OrderItem(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
        #endregion
    }
}