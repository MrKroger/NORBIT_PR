using System;
using System.Text;

namespace Practice.Tasks
{
    /// <summary>
    /// Класс для выполнения финансовых расчетов, связанных с вычислением сложных процентов по вкладу.
    /// </summary>
    public class HardPercent
    {
        #region Точка входа
        /// <summary>
        /// Точка входа в программу. Запрашивает данные у пользователя и выводит результат расчета.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("--- Расчет сложных процентов ---");
            
            try
            {
                decimal initialDeposit = InputDecimal("Введите начальный вклад: ");
                int years = InputInt("Введите количество лет: ");
                decimal interestRate = InputDecimal("Введите годовую процентную ставку: ");

                decimal[] reportData = CalculateHardPercent(initialDeposit, years, interestRate);
                string reportText = FormatDepositReport(reportData);

                Console.WriteLine("\nРезультат расчета:");
                Console.WriteLine(reportText);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\nОшибка ввода: {ex.Message}");
            }
            catch (FormatException)
            {
                Console.WriteLine("\nОшибка ввода: Пожалуйста, используйте только числа.");
            }

            Console.ReadLine();
        }
        #endregion

        #region Бизнес-логика
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
            ArgumentOutOfRangeException.ThrowIfNegative(years);
            ArgumentOutOfRangeException.ThrowIfNegative(interestRate);
            ArgumentOutOfRangeException.ThrowIfNegative(initialDeposit);

            decimal[] results = new decimal[years];
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
            StringBuilder stringBuilder = new();

            for (int i = 0; i < report.Length; i++)
            {
                stringBuilder.AppendLine($"Год {i + 1}: {report[i]:f2} руб.");
            }

            return stringBuilder.ToString(); 
        }
        #endregion

        #region Вспомогательные методы ввода
        private static decimal InputDecimal(string prompt)
        {
            Console.Write(prompt);
            return Convert.ToDecimal(Console.ReadLine());
        }

        private static int InputInt(string prompt)
        {
            Console.Write(prompt);
            return Convert.ToInt32(Console.ReadLine());
        }
        #endregion
    }
}