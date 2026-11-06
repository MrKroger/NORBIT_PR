using System;
using System.Text;

/// <summary>
/// Класс для выполнения финансовых расчетов, связанных с вычислением сложных процентов по вкладу.
/// </summary>
class HardPercent
{
    /// <summary>
    /// Вычисляет сумму накоплений на конец каждого года с использованием формулы сложного процента.
    /// </summary>
    /// <param name="initialDeposit">Начальная сумма вклада. Должна быть неотрицательной.</param>
    /// <param name="years">Количество лет для расчета. Должно быть неотрицательным целым числом.</param>
    /// <param name="interestRate">Годовая процентная ставка (например, 10 для 10%). Должна быть неотрицательной.</param>
    /// <returns>Массив десятичных чисел (decimal), где каждый элемент — сумма на конец соответствующего года.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если хотя бы один из числовых параметров имеет отрицательное значение.</exception>
    static decimal[] CalculateHardPercent(decimal initialDeposit, int years, decimal interestRate)
    {
        // Защита от некорректного ввода (значений меньше нуля)
        ArgumentOutOfRangeException.ThrowIfNegative(years);
        ArgumentOutOfRangeException.ThrowIfNegative(interestRate);
        ArgumentOutOfRangeException.ThrowIfNegative(initialDeposit);

        decimal[] results = new decimal[years];

        // Вычисляем множитель. Например, при ставке 10% множитель будет 1 + 0.1 = 1.1
        decimal percentageMultiplier = 1 + (interestRate / 100.0m); 
        decimal currentDeposit = initialDeposit;

        for (int i = 0; i < years; i++)
        {
            currentDeposit *= percentageMultiplier;
            results[i] = currentDeposit;
        }

        return results;
    }

    /// <summary>
    /// Форматирует массив с результатами расчетов в читаемую строку для вывода в консоль.
    /// </summary>
    /// <param name="report">Массив накоплений по годам, полученный из метода CalculateHardPercent.</param>
    /// <returns>Строка, содержащая отчет по каждому году, разделенный переносом строки.</returns>
    private static string FormatDepositReport(decimal[] report)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < report.Length; i++)
        {
            // Добавляем строку с форматированием до 2 знаков после запятой
            sb.AppendLine($"Год {i + 1}: {report[i]:f2} руб.");
        }

        return sb.ToString(); 
    }

    /// <summary>
    /// Точка входа в программу. Запрашивает данные у пользователя и выводит результат расчета.
    /// </summary>
    static void Main(string[] args)
    {
        Console.Write("Введите начальный вклад: ");
        decimal initialDeposit = Convert.ToDecimal(Console.ReadLine());

        Console.Write("Введите количество лет: ");
        int years = Convert.ToInt32(Console.ReadLine());

        Console.Write("Введите годовую процентную ставку: ");
        decimal interestRate = Convert.ToDecimal(Console.ReadLine());

        try
        {
            // Вызов бизнес-логики для расчета
            decimal[] reportData = CalculateHardPercent(initialDeposit, years, interestRate);

            Console.WriteLine("\nРезультат расчета:");
            
            // Форматирование и вывод данных
            string reportText = FormatDepositReport(reportData);
            Console.WriteLine(reportText);
        }
        catch (ArgumentException ex)
        {
            // Перехватываем ошибку (сюда попадут исключения ArgumentOutOfRangeException)
            Console.WriteLine($"\nОшибка ввода: {ex.Message}");
        }

        Console.ReadLine();
    }
}