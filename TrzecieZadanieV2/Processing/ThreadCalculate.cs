using System.Diagnostics;
using taskThird.Models;
using taskThird.Utils;

namespace taskThird.Processing
{
    /// <summary>
    /// Implementacja przetwarzania obliczeń za pomocą klasy Thread.
    /// </summary>
    public class ThreadCalculate : ICalculate
    {
        /// <summary>
        /// Wykonuje obliczenia całki dla podanych przedziałów równolegle z użyciem Thread.
        /// </summary>
        /// <param name="func">Funkcja matematyczna do całkowania.</param>
        /// <param name="intervals">Lista przedziałów dla obliczeń.</param>
        /// <param name="steps">Liczba kroków aproksymacji trapezowej.</param>
        /// <param name="token">Token do obsługi anulowania obliczeń.</param>
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

                    results[index] = MainCalculator.Calculate(func, intervals[index].Start, intervals[index].End, steps, token, progress =>
                    {
                        Console.WriteLine($"Przedział {index + 1} ({intervals[index].Start}, {intervals[index].End}): {progress}% ukończono (Thread).");
                    });

                    stopwatch.Stop(); // Zatrzymanie stopera dla wątku
                    Console.WriteLine($"Czas działania wątku dla przedziału {index + 1} ({intervals[index].Start}, {intervals[index].End}): {stopwatch.ElapsedMilliseconds} ms");
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start(); // Uruchomienie wątku
            }

            foreach (var thread in threads)
            {
                thread.Join(); // Oczekiwanie na zakończenie wątku
            }

            // Wyświetlenie wyników
            Console.WriteLine("Podsumowanie:");
            for (int i = 0; i < intervals.Count; i++)
            {
                Console.WriteLine($"Przedział {intervals[i]}: wynik = {results[i]:F4} (Thread).");
            }
        }
    }
}
