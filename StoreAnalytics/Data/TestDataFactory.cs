namespace Norbit.StoreAnalytics.Data;

using Norbit.StoreAnalytics.Models;

/// <summary>
/// Фабрика фиксированных тестовых данных. Даты зашиты явно,
/// чтобы результат отчёта был воспроизводимым.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Категории товаров. Категория «Игрушки» намеренно оставлена без товаров.
    /// </summary>
    public static IReadOnlyList<Category> CreateCategories() => new[]
    {
        new Category(1, "Электроника"),
        new Category(2, "Книги"),
        new Category(3, "Одежда"),
        new Category(4, "Спорт"),
        new Category(5, "Игрушки")
    };

    /// <summary>
    /// Поставщики. Поставщик «ООО ПустойПоставщик» намеренно оставлен без товаров.
    /// </summary>
    public static IReadOnlyList<Supplier> CreateSuppliers() => new[]
    {
        new Supplier(1, "ООО ТехноПоставка", 4.5m),
        new Supplier(2, "ИП Иванов", 4.2m),
        new Supplier(3, "ООО КнигоМир", 4.8m),
        new Supplier(4, "ООО СпортИнвентарь", 4.1m),
        new Supplier(5, "ООО МоднаяОдежда", 3.9m),
        new Supplier(6, "ООО ПустойПоставщик", 4.0m)
    };

    /// <summary>
    /// Товары магазина. Набор покрывает все пограничные случаи задания:
    /// товары без поставщика, с нулевым остатком, неактивные, с одинаковой
    /// ценой в одной категории, никогда не купленные и т. д.
    /// </summary>
    public static IReadOnlyList<Product> CreateProducts() => new[]
    {
        // Электроника.
        new Product(1, "Смартфон", 1, 1, 50_000m, 10, true, new DateTime(2026, 1, 1)),
        new Product(2, "Ноутбук", 1, 1, 80_000m, 3, true, new DateTime(2026, 1, 2)),
        new Product(3, "Наушники", 1, 1, 5_000m, 25, true, new DateTime(2026, 1, 3)),
        // Остаток 0, но товар активен — должен отсекаться фильтром каталога.
        new Product(4, "Планшет", 1, 1, 30_000m, 0, true, new DateTime(2026, 1, 4)),
        new Product(5, "Умные часы", 1, 2, 20_000m, 8, true, new DateTime(2026, 1, 5)),
        new Product(19, "Микроволновка", 1, 1, 12_000m, 4, true, new DateTime(2026, 1, 6)),
        new Product(20, "Тостер", 1, 1, 4_000m, 11, true, new DateTime(2026, 1, 7)),

        // Книги.
        new Product(6, "Роман \"Война и мир\"", 2, 3, 1_500m, 20, true, new DateTime(2026, 1, 8)),
        new Product(7, "Учебник C#", 2, 3, 2_500m, 15, true, new DateTime(2026, 1, 9)),
        // Одинаковая цена с товаром 6 в той же категории, остаток 0.
        new Product(8, "Сборник рассказов", 2, 3, 1_500m, 0, true, new DateTime(2026, 1, 10)),
        new Product(9, "Энциклопедия", 2, 3, 5_000m, 5, true, new DateTime(2026, 1, 11)),
        // Никогда не покупается — отсутствует в отчёте по продажам.
        new Product(21, "Блокнот", 2, 3, 200m, 80, true, new DateTime(2026, 1, 12)),
        new Product(22, "Ручка", 2, 3, 100m, 200, true, new DateTime(2026, 1, 13)),

        // Одежда.
        new Product(10, "Футболка", 3, 5, 1_200m, 40, true, new DateTime(2026, 1, 14)),
        new Product(11, "Джинсы", 3, 5, 3_000m, 12, true, new DateTime(2026, 1, 15)),
        // Товар без поставщика.
        new Product(12, "Куртка", 3, null, 8_000m, 6, true, new DateTime(2026, 1, 16)),
        new Product(13, "Носки", 3, 5, 300m, 100, true, new DateTime(2026, 1, 17)),

        // Спорт.
        // Одинаковая цена с товаром 11 (разные категории), остаток 0.
        new Product(14, "Гантель", 4, 4, 3_000m, 0, true, new DateTime(2026, 1, 18)),
        new Product(15, "Скакалка", 4, 4, 500m, 50, true, new DateTime(2026, 1, 19)),
        new Product(16, "Коврик для йоги", 4, 4, 1_500m, 7, true, new DateTime(2026, 1, 20)),
        // Неактивный товар.
        new Product(17, "Беговая дорожка", 4, 4, 45_000m, 2, false, new DateTime(2026, 1, 21)),
        new Product(18, "Мяч", 4, 4, 900m, 30, true, new DateTime(2026, 1, 22))
    };

    /// <summary>
    /// Заказы магазина. Товар «Смартфон» встречается в заказах 1, 2, 5 и 10,
    /// что позволяет проверить Distinct() при подсчёте уникальных заказов.
    /// </summary>
    public static IReadOnlyList<Order> CreateOrders() => new[]
    {
        new Order(1, 1, new DateTime(2026, 1, 15), new[]
        {
            new OrderItem(1, 2, 50_000m),
            new OrderItem(3, 1, 5_000m)
        }),
        new Order(2, 2, new DateTime(2026, 1, 20), new[]
        {
            new OrderItem(1, 1, 50_000m),
            new OrderItem(6, 3, 1_500m)
        }),
        new Order(3, 3, new DateTime(2026, 1, 25), new[]
        {
            new OrderItem(2, 1, 80_000m),
            new OrderItem(7, 2, 2_500m),
            new OrderItem(10, 4, 1_200m)
        }),
        new Order(4, 1, new DateTime(2026, 2, 1), new[]
        {
            new OrderItem(11, 2, 3_000m),
            new OrderItem(15, 1, 500m)
        }),
        new Order(5, 4, new DateTime(2026, 2, 5), new[]
        {
            new OrderItem(1, 1, 50_000m),
            new OrderItem(14, 2, 3_000m),
            new OrderItem(16, 1, 1_500m),
            new OrderItem(18, 3, 900m)
        }),
        new Order(6, 5, new DateTime(2026, 2, 10), new[]
        {
            new OrderItem(6, 1, 1_500m),
            new OrderItem(9, 1, 5_000m),
            new OrderItem(13, 5, 300m),
            new OrderItem(20, 1, 4_000m)
        }),
        new Order(7, 6, new DateTime(2026, 2, 15), new[]
        {
            new OrderItem(3, 2, 5_000m),
            new OrderItem(5, 1, 20_000m),
            new OrderItem(12, 1, 8_000m)
        }),
        new Order(8, 7, new DateTime(2026, 2, 20), new[]
        {
            new OrderItem(15, 2, 500m),
            new OrderItem(18, 2, 900m),
            new OrderItem(19, 1, 12_000m)
        }),
        new Order(9, 2, new DateTime(2026, 2, 25), new[]
        {
            new OrderItem(2, 1, 80_000m),
            new OrderItem(11, 1, 3_000m)
        }),
        new Order(10, 8, new DateTime(2026, 3, 1), new[]
        {
            new OrderItem(1, 1, 50_000m),
            new OrderItem(6, 2, 1_500m)
        })
    };
}