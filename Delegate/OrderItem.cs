namespace SmartOrderProcessor
{
    /// <summary>
    /// Товарная позиция в заказе.
    /// </summary>
    public sealed class OrderItem
    {
        /// <summary>
        /// Инициализирует новый экземпляр товарной позиции.
        /// </summary>
        /// <param name="name">Название товара.</param>
        /// <param name="price">Цена за единицу.</param>
        /// <param name="quantity">Количество.</param>
        public OrderItem(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

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
    }
}