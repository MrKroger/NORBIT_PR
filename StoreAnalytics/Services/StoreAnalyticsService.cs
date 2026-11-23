namespace Norbit.StoreAnalytics.Services;

using Norbit.StoreAnalytics.Models;

/// <summary>
/// Сервис аналитики интернет-магазина на основе LINQ to Objects.
/// </summary>
public static class StoreAnalyticsService
{
    private const decimal TaxMultiplier = 1.2m;
    private const int LowStockThreshold = 5;
    private const int TopSellingCount = 5;
    private const decimal PriceRangeStep = 500m;
    private const decimal HighPriceThreshold = 100_000m;

    /// <summary>
    /// Строит итоговый отчёт по магазину.
    /// </summary>
    /// <param name="categories">Все категории магазина.</param>
    /// <param name="suppliers">Все поставщики магазина.</param>
    /// <param name="products">Все товары магазина.</param>
    /// <param name="orders">Все заказы магазина.</param>
    /// <param name="minPrice">Нижняя граница цены (включительно) или <c>null</c>.</param>
    /// <param name="maxPrice">Верхняя граница цены (включительно) или <c>null</c>.</param>
    /// <param name="categoryId">Идентификатор категории для фильтрации или <c>null</c>.</param>
    /// <param name="searchText">Подстрока для поиска по названию или <c>null</c>.</param>
    /// <param name="pageNumber">Номер страницы (≥ 1).</param>
    /// <param name="pageSize">Размер страницы (≥ 1).</param>
    /// <returns>Готовый отчёт.</returns>
    public static StoreAnalyticsReport BuildReport(
        IEnumerable<Category> categories,
        IEnumerable<Supplier> suppliers,
        IEnumerable<Product> products,
        IEnumerable<Order> orders,
        decimal? minPrice,
        decimal? maxPrice,
        int? categoryId,
        string? searchText,
        int pageNumber,
        int pageSize)
    {
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(suppliers);
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(orders);

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                pageNumber,
                "Номер страницы должен быть больше либо равен 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                "Размер страницы должен быть больше либо равен 1.");
        }

        if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
        {
            throw new ArgumentException(
                $"Минимальная цена ({minPrice.Value}) не может быть больше максимальной ({maxPrice.Value}).",
                nameof(minPrice));
        }

        // Материализуем входные последовательности один раз: их нужно перебирать
        // многократно (валидация, метрики, отчёты). Это обоснованная материализация,
        // а не «промежуточный ToList() ради ToList()».
        IReadOnlyList<Product> productList = products as IReadOnlyList<Product> ?? products.ToList();
        IReadOnlyList<Category> categoryList = categories as IReadOnlyList<Category> ?? categories.ToList();
        IReadOnlyList<Supplier> supplierList = suppliers as IReadOnlyList<Supplier> ?? suppliers.ToList();
        IReadOnlyList<Order> orderList = orders as IReadOnlyList<Order> ?? orders.ToList();

        ValidateProducts(productList);
        ValidateOrders(orderList);

        IReadOnlyList<ProductCatalogItem> catalogPage = BuildCatalogPage(
            productList,
            categoryList,
            supplierList,
            minPrice,
            maxPrice,
            categoryId,
            searchText,
            pageNumber,
            pageSize);

        IReadOnlyList<CategoryStockReport> categoryReports =
            BuildCategoryReports(categoryList, productList);

        IReadOnlyList<ProductSalesReport> topSellingProducts =
            BuildTopSellingProducts(orderList, productList);

        int totalProducts = productList.Count;
        int activeProducts = productList.Count(product => product.IsActive);
        int outOfStockProducts = productList.Count(product => product.StockQuantity == 0);
        decimal totalStockValue = productList.Sum(product => product.Price * product.StockQuantity);

        decimal? averageActiveProductPrice = activeProducts == 0
            ? null
            : productList
                .Where(product => product.IsActive)
                .Average(product => product.Price);

        RunDiagnosticChecks(productList, categoryList);

        return new StoreAnalyticsReport(
            TotalProducts: totalProducts,
            ActiveProducts: activeProducts,
            OutOfStockProducts: outOfStockProducts,
            TotalStockValue: totalStockValue,
            AverageActiveProductPrice: averageActiveProductPrice,
            CatalogPage: catalogPage,
            Categories: categoryReports,
            TopSellingProducts: topSellingProducts);
    }

    /// <summary>
    /// Строит вторую группировку товаров по составному ключу
    /// «категория + ценовой диапазон с шагом 500 рублей».
    /// Возвращает плоский список, не входящий в основной отчёт.
    /// </summary>
    public static IReadOnlyList<PriceRangeGroup> BuildPriceRangeGroups(
        IEnumerable<Category> categories,
        IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(products);

        IReadOnlyList<Category> categoryList =
            categories as IReadOnlyList<Category> ?? categories.ToList();
        IReadOnlyList<Product> productList =
            products as IReadOnlyList<Product> ?? products.ToList();

        // Query syntax: группировка по составному ключу
        // (название категории + нижняя граница диапазона).
        var query =
            from product in productList
            join category in categoryList on product.CategoryId equals category.Id
            group product by new
            {
                category.Name,
                LowerBound = decimal.Floor(product.Price / PriceRangeStep) * PriceRangeStep
            }
            into rangeGroup
            orderby rangeGroup.Key.Name, rangeGroup.Key.LowerBound
            select new PriceRangeGroup(
                CategoryName: rangeGroup.Key.Name,
                LowerBound: rangeGroup.Key.LowerBound,
                UpperBound: rangeGroup.Key.LowerBound + PriceRangeStep,
                ProductCount: rangeGroup.Count(),
                AveragePrice: rangeGroup.Average(product => product.Price));

        return query.ToList();
    }

    /// <summary>
    /// Возвращает текстовые пояснения о различиях
    /// IEnumerable&lt;T&gt;, IQueryable&lt;T&gt;, AsEnumerable() и AsQueryable(),
    /// а также демонстрирует отложенное выполнение и материализованный снимок.
    /// </summary>
    public static IReadOnlyList<string> ExplainQueryInterfaces(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        IReadOnlyList<Product> productList =
            products as IReadOnlyList<Product> ?? products.ToList();

        List<string> notes = new List<string>();

        // Отложенный запрос: Where не выполняется до перечисления.
        IEnumerable<Product> deferredQuery = productList.Where(product => product.Price > 0m);
        notes.Add("Отложенный запрос создан; Where не выполнился до перечисления.");

        // Материализация через ToList — снимок, где Where выполнился один раз.
        List<Product> snapshot = deferredQuery.ToList();
        notes.Add($"Материализованный снимок (ToList): {snapshot.Count} товаров.");

        // AsQueryable() над List<T>: создаётся EnumerableQuery<T>,
        // вызовы Queryable.* транслируются в Enumerable.* — обработка идёт в памяти,
        // SQL-запрос не формируется.
        IQueryable<Product> queryable = productList.AsQueryable();
        int activeViaQueryable = queryable.Count(product => product.IsActive);
        notes.Add(
            $"AsQueryable(): провайдер EnumerableQuery, активных товаров {activeViaQueryable} " +
            "(в памяти, без SQL).");

        // AsEnumerable() «спускает» запрос обратно к LINQ to Objects.
        IEnumerable<Product> enumerable = queryable.AsEnumerable();
        int activeViaEnumerable = enumerable.Count(product => product.IsActive);
        notes.Add(
            $"AsEnumerable(): переключение на LINQ to Objects, активных товаров {activeViaEnumerable}.");

        return notes;
    }

    private static IReadOnlyList<ProductCatalogItem> BuildCatalogPage(
        IReadOnlyList<Product> products,
        IReadOnlyList<Category> categories,
        IReadOnlyList<Supplier> suppliers,
        decimal? minPrice,
        decimal? maxPrice,
        int? categoryId,
        string? searchText,
        int pageNumber,
        int pageSize)
    {
        // Начинаем с полного набора. ToList() здесь намеренно НЕ вызываем:
        // важно сохранить отложенное выполнение, чтобы Where/OrderBy/Skip/Take
        // слились в один конвейер и выполнились только при материализации страницы.
        // Преждевременный ToList() до фильтров означал бы прогон по всем товарам
        // и материализацию даже тех, что не попадут в результат.
        IEnumerable<Product> filteredProducts = products;

        filteredProducts = filteredProducts.Where(product => product.IsActive);

        if (minPrice.HasValue)
        {
            decimal minPriceValue = minPrice.Value;
            filteredProducts = filteredProducts.Where(product => product.Price >= minPriceValue);
        }

        if (maxPrice.HasValue)
        {
            decimal maxPriceValue = maxPrice.Value;
            filteredProducts = filteredProducts.Where(product => product.Price <= maxPriceValue);
        }

        if (categoryId.HasValue)
        {
            int categoryIdValue = categoryId.Value;
            filteredProducts = filteredProducts.Where(product => product.CategoryId == categoryIdValue);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string searchTextValue = searchText.Trim();
            filteredProducts = filteredProducts.Where(product =>
                product.Name.Contains(searchTextValue, StringComparison.OrdinalIgnoreCase));
        }

        filteredProducts = filteredProducts.Where(product => product.StockQuantity > 0);

        // Левое соединение товаров с категориями и поставщиками через query syntax.
        // DefaultIfEmpty() гарантирует, что товар без категории или без поставщика
        // не выпадет из результата: вместо отсутствующей строки появится default (null).
        var catalogQuery =
            from product in filteredProducts
            join category in categories on product.CategoryId equals category.Id into categoryGroup
            from category in categoryGroup.DefaultIfEmpty()
            join supplier in suppliers on product.SupplierId equals (int?)supplier.Id into supplierGroup
            from supplier in supplierGroup.DefaultIfEmpty()
            select new ProductCatalogItem(
                Id: product.Id,
                Name: product.Name,
                CategoryName: category is null ? "Неизвестная категория" : category.Name,
                SupplierName: supplier is null ? "Не указан" : supplier.Name,
                Price: product.Price,
                PriceWithTax: decimal.Round(
                    product.Price * TaxMultiplier,
                    2,
                    MidpointRounding.AwayFromZero),
                StockStatus: GetStockStatus(product.StockQuantity),
                IsActive: product.IsActive);

        // OrderBy обязателен перед Skip/Take: без явной сортировки LINQ to Objects
        // вернёт элементы в порядке источника, и страницы будут «плавать» между запусками.
        // ThenBy(Id) делает порядок устойчивым, если цены и названия совпадают:
        // при пагинации это исключает дублирование и потерю элементов между страницами.
        return catalogQuery
            .OrderBy(item => item.Price)
            .ThenBy(item => item.Name)
            .ThenBy(item => item.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private static string GetStockStatus(int stockQuantity)
    {
        if (stockQuantity == 0)
        {
            return "Нет в наличии";
        }

        if (stockQuantity <= LowStockThreshold)
        {
            return "Мало осталось";
        }

        return "В наличии";
    }

    private static IReadOnlyList<CategoryStockReport> BuildCategoryReports(
        IReadOnlyList<Category> categories,
        IReadOnlyList<Product> products)
    {
        // GroupJoin — левое соединение: категории без товаров дадут пустую группу.
        var query =
            from category in categories
            join product in products on category.Id equals product.CategoryId into categoryProducts
            orderby category.Name
            select BuildCategoryStockReport(category.Name, categoryProducts);

        return query.ToList();
    }

    private static CategoryStockReport BuildCategoryStockReport(
        string categoryName,
        IEnumerable<Product> categoryProducts)
    {
        // Обоснованная материализация: коллекция будет перебрана несколько раз
        // (Count, Sum, Average, Min, Max). Повторное перечисление IGrouping
        // допустимо, но локальный ToList делает намерение явным.
        IReadOnlyList<Product> productList =
            categoryProducts as IReadOnlyList<Product> ?? categoryProducts.ToList();

        if (productList.Count == 0)
        {
            return new CategoryStockReport(
                CategoryName: categoryName,
                ProductCount: 0,
                ActiveProductCount: 0,
                InStockProductCount: 0,
                TotalStockQuantity: 0,
                TotalStockValue: 0m,
                AveragePrice: null,
                MinPrice: null,
                MaxPrice: null);
        }

        // Приведение к decimal? перед Average/Min/Max позволяет безопасно
        // вернуть null для пустых последовательностей — здесь защита на будущее.
        return new CategoryStockReport(
            CategoryName: categoryName,
            ProductCount: productList.Count,
            ActiveProductCount: productList.Count(product => product.IsActive),
            InStockProductCount: productList.Count(product => product.StockQuantity > 0),
            TotalStockQuantity: productList.Sum(product => product.StockQuantity),
            TotalStockValue: productList.Sum(product => product.Price * product.StockQuantity),
            AveragePrice: productList.Select(product => (decimal?)product.Price).Average(),
            MinPrice: productList.Select(product => (decimal?)product.Price).Min(),
            MaxPrice: productList.Select(product => (decimal?)product.Price).Max());
    }

    private static IReadOnlyList<ProductSalesReport> BuildTopSellingProducts(
        IReadOnlyList<Order> orders,
        IReadOnlyList<Product> products)
    {
        // SelectMany «разворачивает» заказы в плоскую последовательность строк.
        // Select вернул бы IEnumerable<IEnumerable<OrderItem>>, и группировать
        // по товарам без дополнительной обработки было бы нельзя.
        var salesLines =
            from order in orders
            from item in order.Items
            select new
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                ProductId = item.ProductId,
                item.Quantity,
                item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            };

        // Внутреннее соединение с товарами. Товар, который ни разу не покупали,
        // в salesLines отсутствует, поэтому в результат не попадёт — это
        // ожидаемое поведение внутреннего Join. Чтобы включить непроданные товары,
        // понадобилось бы левое соединение с DefaultIfEmpty() и Sum/Count = 0.
        var joined =
            from line in salesLines
            join product in products on line.ProductId equals product.Id
            select new { Line = line, Product = product };

        var groupedReports =
            from entry in joined
            group entry by new { entry.Product.Id, entry.Product.Name } into salesGroup
            select new ProductSalesReport(
                ProductId: salesGroup.Key.Id,
                ProductName: salesGroup.Key.Name,
                TotalQuantity: salesGroup.Sum(entry => entry.Line.Quantity),
                // Distinct() обязателен: если один товар встречается в заказе
                // несколькими строками, без него OrdersCount посчитал бы строки,
                // а не уникальные заказы.
                OrdersCount: salesGroup
                    .Select(entry => entry.Line.OrderId)
                    .Distinct()
                    .Count(),
                Revenue: salesGroup.Sum(entry => entry.Line.LineTotal));

        return groupedReports
            .OrderByDescending(report => report.Revenue)
            .ThenByDescending(report => report.TotalQuantity)
            .ThenBy(report => report.ProductName)
            .ThenBy(report => report.ProductId)
            .Take(TopSellingCount)
            .ToList();
    }

    private static void ValidateProducts(IReadOnlyList<Product> products)
    {
        foreach (Product product in products)
        {
            if (product.Price < 0m)
            {
                throw new ArgumentException(
                    $"Товар '{product.Name}' (Id={product.Id}) имеет отрицательную цену ({product.Price}).",
                    nameof(products));
            }

            if (product.StockQuantity < 0)
            {
                throw new ArgumentException(
                    $"Товар '{product.Name}' (Id={product.Id}) имеет отрицательный остаток ({product.StockQuantity}).",
                    nameof(products));
            }
        }
    }

    private static void ValidateOrders(IReadOnlyList<Order> orders)
    {
        foreach (Order order in orders)
        {
            foreach (OrderItem item in order.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException(
                        $"Заказ {order.Id} содержит строку товара {item.ProductId} " +
                        $"с неположительным количеством ({item.Quantity}).",
                        nameof(orders));
                }

                if (item.UnitPrice < 0m)
                {
                    throw new ArgumentException(
                        $"Заказ {order.Id} содержит строку товара {item.ProductId} " +
                        $"с отрицательной ценой ({item.UnitPrice}).",
                        nameof(orders));
                }
            }
        }
    }

    /// <summary>
    /// Демонстрирует операторы Any, All, FirstOrDefault, Single.
    /// Результаты зафиксированы в комментариях рядом с вызовами.
    /// </summary>
    private static void RunDiagnosticChecks(
        IReadOnlyList<Product> products,
        IReadOnlyList<Category> categories)
    {
        // Any(): есть ли товары с нулевым остатком?
        // В тестовых данных — да (Id = 4, 8, 14). Результат: true.
        bool hasOutOfStockProducts = products.Any(product => product.StockQuantity == 0);

        // All(): все ли цены неотрицательны? После валидации — true.
        bool allPricesNonNegative = products.All(product => product.Price >= 0m);
        if (!allPricesNonNegative)
        {
            throw new InvalidOperationException(
                "All() обнаружил товар с отрицательной ценой, хотя валидация прошла.");
        }

        // FirstOrDefault(): первый товар дороже 100_000m.
        // В тестовых данных такого товара нет → результат null.
        Product? firstHighPriceProduct = products
            .Where(product => product.Price > HighPriceThreshold)
            .FirstOrDefault();

        // Single(): категория по уникальному идентификатору.
        // Категория с Id == 1 («Электроника») в тестовых данных ровно одна.
        Category? firstCategory = categories.Count(category => category.Id == 1) == 1
            ? categories.Single(category => category.Id == 1)
            : null;

        // Явное «касание» переменных, чтобы компилятор не выдал предупреждение
        // о неиспользуемых локальных переменных. Значения не входят в отчёт,
        // но фиксируются как демонстрация операторов.
        _ = hasOutOfStockProducts;
        _ = firstHighPriceProduct;
        _ = firstCategory;
    }
}