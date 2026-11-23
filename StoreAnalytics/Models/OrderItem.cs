namespace Norbit.StoreAnalytics.Models;

/// <summary>
/// Строка заказа: товар, количество и цена на момент оформления.
/// </summary>
public sealed record OrderItem(
    int ProductId,
    int Quantity,
    decimal UnitPrice);