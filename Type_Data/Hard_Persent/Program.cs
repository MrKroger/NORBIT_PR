using System;
using System.Text;

class Program
{
    static string CalculateHardPercent(double initialDeposit, int years, double interestRate)
    {
        StringBuilder result = new StringBuilder();

        double percentageMultiplier = 1 + (interestRate / 100.0);

        double currentDeposit = initialDeposit;

        for (int i = 1; i <= years; i++)
        {
            currentDeposit *= percentageMultiplier;
            result.AppendLine($"Год {i}: {currentDeposit:f2} руб.");
        }

        return result.ToString();
    }

    static void Main(string[] args)
    {
        Console.Write("Введите начальный вклад: ");
        double initialDeposit = Convert.ToDouble(Console.ReadLine());

        Console.Write("Введите количество лет: ");
        int years = Convert.ToInt32(Console.ReadLine());

        Console.Write("Введите годовую процентную ставку: ");
        double interestRate = Convert.ToDouble(Console.ReadLine());

        string report = CalculateHardPercent(initialDeposit, years, interestRate);

        Console.WriteLine("\nРезультат расчета:");
        Console.WriteLine(report);

        Console.ReadLine();
    }
}