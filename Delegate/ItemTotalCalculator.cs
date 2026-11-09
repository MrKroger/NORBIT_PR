namespace SmartOrderProcessor
{
    /// <summary>
    /// Пользовательский делегат для расчета стоимости одной позиции.
    /// </summary>
    /// <param name="item">Исходная товарная позиция.</param>
    /// <param name="transformedUnitPrice">Цена после применения преобразований (скидки, налоги).</param>
    /// <returns>Итоговая стоимость позиции (обычно цена * количество).</returns>
    public delegate decimal ItemTotalCalculator(OrderItem item, decimal transformedUnitPrice);
}