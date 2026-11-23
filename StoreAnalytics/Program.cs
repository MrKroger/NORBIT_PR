namespace Norbit.StoreAnalytics;

using System.Text;
using Norbit.StoreAnalytics.Data;
using Norbit.StoreAnalytics.Models;
using Norbit.StoreAnalytics.Services;

/// <summary>
/// Точка входа демонстрационного приложения «Аналитика интернет-магазина».
/// </summary>
public static class Program
{
    /// <summary>
    /// Запускает консольную демонстрацию: формирует отчёт, печатает каталог,
    /// отчёты по категориям и продажам, а также поясняет различия LINQ-провайдеров.
    /// </summary>
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        IReadOnlyList<Category> categories = TestDataFactory.CreateCategories();
        IReadOnlyList<Supplier> suppliers = TestDataFactory.CreateSuppliers();
        IReadOnlyList<Product> products = TestDataFactory.CreateProducts();
        IReadOnlyList<Order> orders = TestDataFactory.CreateOrders();

        StoreAnalyticsReport report = StoreAnalyticsService.BuildReport(
            categories: categories,
            suppliers: suppliers,
            products: products,
            orders: orders,
            minPrice: 500m,
            maxPrice: 50_000m,
            categoryId: null,
            searchText: null,
            pageNumber: 1,
            pageSize: 10);

        PrintHeader("Общие показатели магазина");
        PrintGeneralMetrics(report);

        PrintHeader("Каталог (фильтр 500–50000, страница 1)");
        PrintCatalog(report.CatalogPage);

        PrintHeader("Отчёт по категориям");
        PrintCategoryReports(report.Categories);

        PrintHeader("Топ-5 товаров по продажам");
        PrintTopSellingProducts(report.TopSellingProducts);

        PrintHeader("Группировка по категориям и ценовым диапазонам");
        PrintPriceRangeGroups(categories, products);

        PrintHeader("IEnumerable / IQueryable / AsEnumerable / AsQueryable");
        PrintQueryInterfaceNotes(products);
    }

    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 70));
    }

    private static void PrintGeneralMetrics(StoreAnalyticsReport report)
    {
        Console.WriteLine($"Всего товаров:          {report.TotalProducts}");
        Console.WriteLine($"Активных товаров:       {report.ActiveProducts}");
        Console.WriteLine($"Нет в наличии:          {report.OutOfStockProducts}");
        Console.WriteLine($"Стоимость запасов:      {report.TotalStockValue:N2} ₽");
        Console.WriteLine(
            $"Средняя цена активных:  {report.AverageActiveProductPrice:N2} ₽");
    }

    private static void PrintCatalog(IReadOnlyList<ProductCatalogItem> catalog)
    {
        if (catalog.Count == 0)
        {
            Console.WriteLine("(Каталог пуст по заданным фильтрам.)");
            return;
        }

        foreach (ProductCatalogItem item in catalog)
        {
            Console.WriteLine(
                $"[{item.Id,3}] {item.Name,-25} " +
                $"кат.: {item.CategoryName,-20} " +
                $"пост.: {item.SupplierName,-22} " +
                $"цена: {item.Price,8:N2} ₽ " +
                $"с НДС: {item.PriceWithTax,10:N2} ₽ " +
                $"статус: {item.StockStatus}");
        }
    }

    private static void PrintCategoryReports(IReadOnlyList<CategoryStockReport> reports)
    {
        foreach (CategoryStockReport report in reports)
        {
            Console.WriteLine(
                $"{report.CategoryName,-15} " +
                $"товаров: {report.ProductCount,2}, " +
                $"активных: {report.ActiveProductCount,2}, " +
                $"в наличии: {report.InStockProductCount,2}, " +
                $"остаток: {report.TotalStockQuantity,4}, " +
                $"запасы: {report.TotalStockValue,12:N2} ₽, " +
                $"сред.: {FormatNullable(report.AveragePrice)}, " +
                $"мин.: {FormatNullable(report.MinPrice)}, " +
                $"макс.: {FormatNullable(report.MaxPrice)}");
        }
    }

    private static void PrintTopSellingProducts(IReadOnlyList<ProductSalesReport> reports)
    {
        foreach (ProductSalesReport report in reports)
        {
            Console.WriteLine(
                $"[{report.ProductId,3}] {report.ProductName,-25} " +
                $"продано: {report.TotalQuantity,4} шт., " +
                $"заказов: {report.OrdersCount,2}, " +
                $"выручка: {report.Revenue,12:N2} ₽");
        }
    }

    private static void PrintPriceRangeGroups(
        IReadOnlyList<Category> categories,
        IReadOnlyList<Product> products)
    {
        IReadOnlyList<PriceRangeGroup> groups =
            StoreAnalyticsService.BuildPriceRangeGroups(categories, products);

        foreach (PriceRangeGroup group in groups)
        {
            Console.WriteLine(
                $"{group.CategoryName,-15} " +
                $"{group.LowerBound,10:N0}–{group.UpperBound,10:N0} ₽: " +
                $"товаров {group.ProductCount,2}, " +
                $"средняя {group.AveragePrice,10:N2} ₽");
        }
    }

    private static void PrintQueryInterfaceNotes(IEnumerable<Product> products)
    {
        IReadOnlyList<string> notes =
            StoreAnalyticsService.ExplainQueryInterfaces(products);

        foreach (string note in notes)
        {
            Console.WriteLine($"• {note}");
        }
    }

    private static string FormatNullable(decimal? value) =>
        value.HasValue ? $"{value.Value:N2} ₽" : "null";
}