using taskThird.Models;
using taskThird.Utils;

using System.Diagnostics;

namespace taskThird.Processing
{
    public class ThreadProcessing : IProcessingMethod
    {
        public void Process(Func<double, double> func, List<Interval> intervals, int steps, CancellationToken token)
        {
            var threads = new List<Thread>();
            var results = new double[intervals.Count];

            for (int i = 0; i < intervals.Count; i++)
            {
                int index = i;
                threads.Add(new Thread(() =>
                {
                    Stopwatch stopwatch = new Stopwatch(); // Stoper dla poszczególnego wątku
                    stopwatch.Start();

                    results[index] = IntegralCalculator.Calculate(func, intervals[index].Start, intervals[index].End, steps, token, progress =>
                    {
                        Console.WriteLine($"Przedział {index + 1} ({intervals[index].Start}, {intervals[index].End}): {progress}% ukończono (Thread).");
                    });

                    stopwatch.Stop(); // Zatrzymanie stopera dla wątku
                    Console.WriteLine($"Czas działania wątku dla przedziału {index + 1} ({intervals[index].Start}, {intervals[index].End}): {stopwatch.ElapsedMilliseconds} ms");
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("Podsumowanie:");
            for (int i = 0; i < intervals.Count; i++)
            {
                Console.WriteLine($"Przedział {intervals[i]}: wynik = {results[i]:F4} (Thread).");
            }
        }
    }
}
