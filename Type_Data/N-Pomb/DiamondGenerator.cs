using System;
using System.Text;

/// <summary>
/// Класс для генерации графических фигур (ромбов) в консоли.
/// </summary>
class DiamondGenerator
{
    /// <summary>
    /// Формирует строковое представление ромба из символов 'X'.
    /// </summary>
    /// <param name="size">Размер ромба (длина диагонали). Должен быть положительным нечетным числом.</param>
    /// <returns>Строка, содержащая нарисованный ромб.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если размер меньше или равен нулю, либо если размер является четным числом.</exception>
    static string DrawDiamond(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        
        // По условию задачи N должно быть нечетным, чтобы был четкий центр
        if (size % 2 == 0) 
            throw new ArgumentOutOfRangeException(nameof(size), "Размер ромба должен быть нечетным числом.");

        StringBuilder sb = new StringBuilder();

        // Находим индекс центра
        int center = size / 2;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // Вычисляем манхэттенское расстояние до центра
                int distance = Math.Abs(center - i) + Math.Abs(center - j);
                
                if (distance == center)
                {
                    sb.Append("X");
                }
                else
                {
                    sb.Append(" ");
                }
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Точка входа в программу. Запрашивает размер ромба у пользователя и выводит его на экран.
    /// </summary>
    static void Main(string[] args)
    {
        Console.Write("Введите N (положительное нечетное число): ");
        int size = Convert.ToInt32(Console.ReadLine());

        try
        {
            // Вызов бизнес-логики для отрисовки
            string diamond = DrawDiamond(size);

            Console.WriteLine("\nРезультат:");
            Console.WriteLine(diamond);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // Перехватываем ошибку валидации
            Console.WriteLine($"\nОшибка ввода: {ex.Message}");
        }
        catch (FormatException)
        {
            // Перехватываем ошибку, если ввели не число (например, букву)
            Console.WriteLine("\nОшибка ввода: Пожалуйста, введите целое число.");
        }

        Console.ReadLine();
    }
}