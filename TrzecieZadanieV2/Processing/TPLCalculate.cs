using System.Diagnostics;
using taskThird.Models;
using taskThird.Utils;

namespace taskThird.Processing
{
    /// <summary>
    /// Implementacja przetwarzania obliczeń za pomocą Task Parallel Library (TPL).
    /// </summary>
    public class TPLCalculate : ICalculate
    {
        /// <summary>
        /// Wykonuje obliczenia całki dla podanych przedziałów równolegle z użyciem TPL.
        /// </summary>
        public void Process(Func<double, double> func, List<Interval> intervals, int steps, CancellationToken token)
        {
            var tasks = intervals.Select((interval, index) => Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch(); // Stoper dla poszczególnego wątku
                stopwatch.Start();

                double result = MainCalculator.Calculate(func, interval.Start, interval.End, steps, token, progress =>
                {
                    Console.WriteLine($"Przedział {index + 1} ({interval.Start}, {interval.End}): {progress}% ukończono (TPL).");
                });

                stopwatch.Stop(); // Zatrzymanie stopera dla wątku
                Console.WriteLine($"Czas działania wątku dla przedziału {index + 1} ({interval.Start}, {interval.End}): {stopwatch.ElapsedMilliseconds} ms");

                return result;
            }, token)).ToArray();

            try
            {
                Task.WaitAll(tasks);
                Console.WriteLine("Podsumowanie:");
                for (int i = 0; i < tasks.Length; i++)
                {
                    Console.WriteLine($"Przedział {intervals[i]}: wynik = {tasks[i].Result:F4} (TPL).");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Obliczenia zostały przerwane (TPL).");
            }
        }
    }
}
