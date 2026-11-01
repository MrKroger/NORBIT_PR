using System;
using System.Text;

class Program
{
    static string CalculateHardPercent(int N)
    {
        StringBuilder result = new StringBuilder();

        int center = N / 2;

        for (int i = 0; i <= N; i++)
        {
            for (int j = 0; j<=N; j++)
            {
                int distance = Math.Abs(center - j)+ Math.Abs(center -i);
                if (distance == center)
                {
                    result.Append("X");
                }
                else
                {
                    result.Append(" ");
                }
            }
            result.AppendLine();
        }

        return result.ToString();
    }

    static void Main(string[] args)
    {
        Console.Write("Введите N: ");
        int N = Convert.ToInt32(Console.ReadLine());

        string report = CalculateHardPercent(N);

        Console.WriteLine(report);

        Console.ReadLine();
    }
}