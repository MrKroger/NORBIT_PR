namespace Norbit.StoreAnalytics.Models;

/// <summary>
/// Категория товаров.
/// </summary>
/// <param name="Id">Уникальный идентификатор категории.</param>
/// <param name="Name">Отображаемое имя категории.</param>
public sealed record Category(
    int Id,
    string Name);