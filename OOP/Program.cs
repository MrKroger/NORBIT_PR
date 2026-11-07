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
            Console.Write("Введите координату X (может быть отрицательной): ");
            double x = Convert.ToDouble(Console.ReadLine());

            Console.Write("Введите координату Y (может быть отрицательной): ");
            double y = Convert.ToDouble(Console.ReadLine());

            Console.Write("Введите ширину (больше нуля): ");
            double width = Convert.ToDouble(Console.ReadLine());

            Console.Write("Введите высоту (больше нуля): ");
            double height = Convert.ToDouble(Console.ReadLine());

            // Создание объекта
            Rectangle rect = new Rectangle(x, y, width, height);

            Console.WriteLine("\n--- Результаты ---");
            Console.WriteLine($"Прямоугольник с координатами ({rect.X}, {rect.Y})");
            Console.WriteLine($"Размеры: {rect.Width} x {rect.Height}");
            Console.WriteLine($"Площадь: {rect.Area:f2}");
            Console.WriteLine($"Периметр: {rect.Perimeter:f2}");
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
}