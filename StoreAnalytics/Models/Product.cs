namespace Norbit.StoreAnalytics.Models;

/// <summary>
/// Товар магазина.
/// </summary>
public sealed record Product(
    int Id,
    string Name,
    int CategoryId,
    int? SupplierId,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedAt);