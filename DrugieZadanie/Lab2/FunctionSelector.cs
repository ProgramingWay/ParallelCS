using System;

// Klasa odpowiedzialna za wybór funkcji przez użytkownika.
public static class FunctionSelector
{
    public static Func<double, double> ChooseFunction()
    {
        Console.WriteLine("Wybierz funkcję do obliczeń:");
        Console.WriteLine("1: y = 2x + 2x^2");
        Console.WriteLine("2: y = 2x^2 + 3");
        Console.WriteLine("3: y = 3x^2 + 2x - 3");
        int choice = int.Parse(Console.ReadLine() ?? "1");

        return choice switch
        {
            1 => (x) => 2 * x + 2 * Math.Pow(x, 2),
            2 => (x) => 2 * Math.Pow(x, 2) + 3,
            3 => (x) => 3 * Math.Pow(x, 2) + 2 * x - 3,
            _ => throw new ArgumentException("Nieprawidłowy wybór funkcji.")
        };
    }
}