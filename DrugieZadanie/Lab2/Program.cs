using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var function = FunctionSelector.ChooseFunction();
        var intervals = IntervalSelector.GetIntervals();

        Console.WriteLine("Podaj liczbę kroków aproksymacji:");
        int steps = int.Parse(Console.ReadLine() ?? "100");

        using var cts = new CancellationTokenSource();
        var results = new ConcurrentDictionary<int, string>();
        IProgress<(int IntervalId, int Progress)> progress = new Progress<(int IntervalId, int Progress)>(data =>
        {
            MessageQueue.Enqueue($"Przedział {data.IntervalId}: Postęp: {data.Progress}%");
        });

        var tasks = intervals.Select(async (interval, index) =>
        {
            var intervalId = index + 1; // Indeks zaczyna się od 1
            var calculator = new CalculationProgress(progress, cts.Token);

            if (cts.Token.IsCancellationRequested)
            {
                results[intervalId] = $"Przedział {intervalId}: obliczenia przerwane.";
                return;
            }

            double result = await calculator.CalculateWithProgress(intervalId, function, interval.Start, interval.End, steps);
            results[intervalId] = $"Przedział {intervalId}: [{interval.Start}, {interval.End}] Wynik: {result}";
        });

        Console.WriteLine("Naciśnij dowolny klawisz, aby zatrzymać...");
        _ = Task.Run(() =>
        {
            Console.ReadKey();
            cts.Cancel();
        });

        await Task.WhenAll(tasks);

        // Wyświetlenie podsumowania
        MessageQueue.Enqueue("Podsumowanie obliczeń:");
        foreach (var result in results.OrderBy(r => r.Key))
        {
            MessageQueue.Enqueue(result.Value);
        }

        MessageQueue.Enqueue("Obliczenia zakończone.");
        MessageQueue.Complete();
        MessageQueue.Flush();
    }
}