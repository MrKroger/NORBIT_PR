using System;
using System.Text;

namespace Practice.Tasks
{
    /// <summary>
    /// Класс для генерации графических фигур (ромбов) в консоли.
    /// </summary>
    public class DiamondGenerator
    {
        #region Точка входа
        /// <summary>
        /// Точка входа в программу. Запрашивает размер ромба у пользователя и выводит его на экран.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("--- Генератор ромба ---");

            try
            {
                int size = InputInt("Введите N (положительное нечетное число): ");
                string diamond = DrawDiamond(size);

                Console.WriteLine("\nРезультат:");
                Console.WriteLine(diamond);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"\nОшибка валидации: {ex.Message}");
            }
            catch (FormatException)
            {
                Console.WriteLine("\nОшибка ввода: Пожалуйста, введите целое число.");
            }

            Console.ReadLine();
        }
        #endregion

        #region Бизнес-логика
        /// <summary>
        /// Формирует строковое представление ромба из символов 'X'.
        /// </summary>
        /// <param name="size">Размер ромба (длина диагонали). Должен быть положительным нечетным числом.</param>
        /// <returns>Строка, содержащая нарисованный ромб.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если размер меньше или равен нулю, либо если размер является четным числом.</exception>
        static string DrawDiamond(int size)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
            
            if (size % 2 == 0) 
                throw new ArgumentOutOfRangeException(nameof(size), "Размер ромба должен быть нечетным числом.");

            StringBuilder stringBuilder = new();
            int center = size / 2;

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    int distance = Math.Abs(center - row) + Math.Abs(center - col);
                    
                    stringBuilder.Append(distance == center ? "X" : " ");
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
        #endregion

        #region Вспомогательные методы ввода
        private static int InputInt(string prompt)
        {
            Console.Write(prompt);
            return Convert.ToInt32(Console.ReadLine());
        }
        #endregion
    }
}