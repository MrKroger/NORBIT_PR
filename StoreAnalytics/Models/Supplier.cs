namespace Norbit.StoreAnalytics.Models;

/// <summary>
/// Поставщик товаров.
/// </summary>
/// <param name="Id">Уникальный идентификатор поставщика.</param>
/// <param name="Name">Наименование поставщика.</param>
/// <param name="Rating">Рейтинг поставщика.</param>
public sealed record Supplier(
    int Id,
    string Name,
    decimal Rating);