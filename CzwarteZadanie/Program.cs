using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ScottPlot;

class Program
{
    static void Main(string[] args)
    {
        ConcurrentBag<(double x, double y)> results = new ConcurrentBag<(double, double)>();
        bool fileSaved = false;
        string fileName = string.Empty;

        Console.WriteLine("Rozpoczynam obliczenia równoległe...");

        // Task do przeprowadzania obliczeń równoległych
        Task computationTask = Task.Run(() =>
        {
            Parallel.For(0, 1000, i =>
            {
                double x = i / 10.0;
                double y = Math.Sin(x); // Przykładowa funkcja: sinus
                results.Add((x, y));
            });
            Console.WriteLine("Obliczenia zakończone.");
        });

        // Task do zbierania nazwy pliku od użytkownika
        Task inputTask = Task.Run(() =>
        {
            while (!fileSaved)
            {
                Console.WriteLine("Podaj nazwę pliku, do którego zapisać wykres (np. wykres.png):");
                fileName = Console.ReadLine();

                if (string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Nazwa pliku nie może być pusta.");
                    continue;
                }

                try
                {
                    if (!computationTask.IsCompleted)
                    {
                        Console.WriteLine("Czekam na zakończenie obliczeń...");
                        computationTask.Wait();
                    }

                    // Generowanie wykresu
                    Console.WriteLine("Generuję wykres...");
                    var plt = new ScottPlot.Plot();

                    var orderedResults = results.OrderBy(r => r.x).ToArray();
                    double[] xs = orderedResults.Select(r => r.x).ToArray();
                    double[] ys = orderedResults.Select(r => r.y).ToArray();

                    plt.Add.Scatter(xs, ys);
                    plt.SavePng(fileName, 800, 600); // Rozdzielczość obrazu

                    // Otwarcie wykresu po zapisaniu
                    var process = new Process();
                    process.StartInfo = new ProcessStartInfo(fileName)
                    {
                        UseShellExecute = true
                    };
                    process.Start();

                    fileSaved = true;
                    Console.WriteLine($"Wykres zapisany i otwarty: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd podczas zapisu pliku: {ex.Message}");
                }
            }
        });

        // Czekaj na zakończenie wszystkich zadań
        Task.WaitAll(computationTask, inputTask);
        Console.WriteLine("Program zakończył działanie.");
    }
}
