using System;

/// <summary>
/// Главный класс программы для демонстрации работы с классом Rectangle.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- Демонстрация работы класса Rectangle ---");

        try
        {
            var x = InputDouble("Введите координату X (может быть отрицательной): ");
            var y = InputDouble("Введите координату Y (может быть отрицательной): ");
            var width = InputDouble("Введите ширину (больше нуля): ");
            var height = InputDouble("Введите высоту (больше нуля): ");

            // Создание объекта
            var rect = new Rectangle(x, y, width, height);

            Console.WriteLine("\n--- Результаты ---");
            // Console.WriteLine($"Прямоугольник с координатами ({rect.X}, {rect.Y})");
            // Console.WriteLine($"Размеры: {rect.Width} x {rect.Height}");
            // Console.WriteLine($"Площадь: {rect.Area:f2}");
            // Console.WriteLine($"Периметр: {rect.Perimeter:f2}");

            Console.WriteLine(rect);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"\nОшибка валидации: Некорректное значение для '{ex.ParamName}'. {ex.Message}");
        }
        catch (FormatException)
        {
            Console.WriteLine("\nОшибка ввода: Пожалуйста, используйте только числа (можно с десятичной точкой).");
        }

        Console.ReadLine();
    }

    /// <summary>
    /// Запрашивает у пользователя число.
    /// </summary>
    /// <param name="prompt">Текст приглашения к вводу, который будет выведен на экран.</param>
    /// <returns>Введенное пользователем число типа double.</returns>
    /// <exception cref="FormatException">Выбрасывается, если введенная строка не является числом.</exception>
    private static double InputDouble(string prompt)
    {
        Console.Write(prompt);
        return Convert.ToDouble(Console.ReadLine());
    }
}