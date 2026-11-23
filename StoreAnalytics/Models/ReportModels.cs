namespace Norbit.StoreAnalytics.Models;

/// <summary>
/// Элемент каталога товаров для отчёта.
/// </summary>
public sealed record ProductCatalogItem(
    int Id,
    string Name,
    string CategoryName,
    string SupplierName,
    decimal Price,
    decimal PriceWithTax,
    string StockStatus,
    bool IsActive);

/// <summary>
/// Отчёт по остаткам одной категории.
/// </summary>
public sealed record CategoryStockReport(
    string CategoryName,
    int ProductCount,
    int ActiveProductCount,
    int InStockProductCount,
    int TotalStockQuantity,
    decimal TotalStockValue,
    decimal? AveragePrice,
    decimal? MinPrice,
    decimal? MaxPrice);

/// <summary>
/// Отчёт по продажам одного товара.
/// </summary>
public sealed record ProductSalesReport(
    int ProductId,
    string ProductName,
    int TotalQuantity,
    int OrdersCount,
    decimal Revenue);

/// <summary>
/// Итоговый аналитический отчёт по магазину.
/// </summary>
public sealed record StoreAnalyticsReport(
    int TotalProducts,
    int ActiveProducts,
    int OutOfStockProducts,
    decimal TotalStockValue,
    decimal? AverageActiveProductPrice,
    IReadOnlyList<ProductCatalogItem> CatalogPage,
    IReadOnlyList<CategoryStockReport> Categories,
    IReadOnlyList<ProductSalesReport> TopSellingProducts);

/// <summary>
/// Группировка товаров по категории и ценовому диапазону.
/// Используется только для дополнительного вывода в консоль.
/// </summary>
public sealed record PriceRangeGroup(
    string CategoryName,
    decimal LowerBound,
    decimal UpperBound,
    int ProductCount,
    decimal AveragePrice);