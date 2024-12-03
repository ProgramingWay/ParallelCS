using System;
using System.Collections.Generic;

// Klasa odpowiedzialna za wybór przedziałów przez użytkownika.

public static class IntervalSelector
{
    public static List<(double Start, double End)> GetIntervals()
    {
        Console.WriteLine("Podaj liczbę przedziałów:");
        int count = int.Parse(Console.ReadLine() ?? "1");
        var intervals = new List<(double Start, double End)>();

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"Podaj początek przedziału {i + 1}:");
            double start = double.Parse(Console.ReadLine() ?? "0");
            Console.WriteLine($"Podaj koniec przedziału {i + 1}:");
            double end = double.Parse(Console.ReadLine() ?? "1");
            intervals.Add((start, end));
        }

        return intervals;
    }
}