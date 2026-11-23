namespace Norbit.StoreAnalytics.Models;

/// <summary>
/// Заказ покупателя.
/// </summary>
public sealed record Order(
    int Id,
    int CustomerId,
    DateTime CreatedAt,
    IReadOnlyList<OrderItem> Items);