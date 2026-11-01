using System;

class Hard_Persent
{
    static double[] CalculateHardPercent(double initialDeposit, int years, double interestRate)
    {
        double[] results = new double[years];

        double percentageMultiplier = 1 + (interestRate / 100.0);
        double currentDeposit = initialDeposit;

        for (int i = 0; i < years; i++)
        {
            currentDeposit *= percentageMultiplier;
            results[i] = currentDeposit;
        }

        return results;
    }

    static void Main(string[] args)
    {
        Console.Write("Введите начальный вклад: ");
        double initialDeposit = Convert.ToDouble(Console.ReadLine());

        Console.Write("Введите количество лет: ");
        int years = Convert.ToInt32(Console.ReadLine());

        Console.Write("Введите годовую процентную ставку: ");
        double interestRate = Convert.ToDouble(Console.ReadLine());

        double[] report = CalculateHardPercent(initialDeposit, years, interestRate);

        Console.WriteLine("\nРезультат расчета:");
        for (int i = 0; i < report.Length; i++)
        {
            Console.WriteLine($"Год {i + 1}: {report[i]:f2} руб.");
        }

        Console.ReadLine();
    }
}