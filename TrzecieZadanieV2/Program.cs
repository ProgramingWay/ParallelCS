using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using taskThird.Processing;
using taskThird.Models;
using System.Diagnostics;

namespace taskThird
{
    /// <summary>
    /// Główna klasa aplikacji, zarządza interakcją użytkownika, wyborem funkcji, metodą przetwarzania
    /// oraz uruchamia obliczenia całki dla podanych przedziałów.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch totalStopwatch = new Stopwatch(); // Miernik czasu działania całej aplikacji.
            totalStopwatch.Start();

            // Wybór funkcji matematycznej
            Console.WriteLine("Wybierz funkcję do obliczenia całki:");
            Console.WriteLine("1. y = 2x + 2x^2");
            Console.WriteLine("2. y = 2x^2 + 3");
            Console.WriteLine("3. y = 3x^2 + 2x - 3");

            string userInput = Console.ReadLine();
            int choice = ParseInputToInt(userInput, 1, 3);

            Func<double, double> selectedFunction = SelectFunction(choice);

            // Pobranie liczby przedziałów
            Console.WriteLine("Podaj liczbę przedziałów (maksymalnie 100):");
            userInput = Console.ReadLine();
            int intervalCount = ParseInputToInt(userInput, 1, 100);

            var intervals = GetIntervals(intervalCount);

            // Wybór metody przetwarzania
            Console.WriteLine("Wybierz metodę przetwarzania:");
            Console.WriteLine("1. TPL");
            Console.WriteLine("2. Thread");

            userInput = Console.ReadLine();
            int methodChoice = ParseInputToInt(userInput, 1, 2);
            ICalculate processingMethod = ChooseProcessingMethod(methodChoice);

            // Obsługa anulowania obliczeń
            var cts = new CancellationTokenSource();
            Console.WriteLine("Naciśnij dowolny klawisz, aby anulować obliczenia.");
            Task.Run(() =>
            {
                Console.ReadKey(true);
                cts.Cancel();
            });

            // Uruchomienie procesu obliczeń
            processingMethod.Process(selectedFunction, intervals, 1000, cts.Token);

            totalStopwatch.Stop(); // Zatrzymanie stopera
            Console.WriteLine($"Całkowity czas działania aplikacji: {totalStopwatch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Parsuje wejście użytkownika na liczbę całkowitą i sprawdza, czy mieści się w zadanym zakresie.
        /// </summary>
        static int ParseInputToInt(string input, int min, int max)
        {
            if (!int.TryParse(input, out int result) || result < min || result > max)
            {
                throw new ArgumentException($"Wartość musi być liczbą od {min} do {max}.");
            }
            return result;
        }

        /// <summary>
        /// Zwraca funkcję matematyczną na podstawie wyboru użytkownika.
        /// </summary>
        static Func<double, double> SelectFunction(int choice)
        {
            return choice switch
            {
                1 => x => 2 * x + 2 * Math.Pow(x, 2),
                2 => x => 2 * Math.Pow(x, 2) + 3,
                3 => x => 3 * Math.Pow(x, 2) + 2 * x - 3,
                _ => throw new InvalidOperationException("Nieprawidłowy wybór funkcji.")
            };
        }

        /// <summary>
        /// Pobiera listę przedziałów od użytkownika.
        /// </summary>
        static List<Interval> GetIntervals(int count)
        {
            var intervals = new List<Interval>();
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"Przedział {i + 1} - podaj początek i koniec (oddzielone spacją):");
                string input = Console.ReadLine();
                string[] parts = input?.Split();
                if (parts == null || parts.Length != 2)
                {
                    throw new ArgumentException("Podaj dokładnie dwie liczby oddzielone spacją.");
                }

                if (!double.TryParse(parts[0], out double start) || !double.TryParse(parts[1], out double end))
                {
                    throw new ArgumentException("Wartości muszą być liczbami.");
                }

                intervals.Add(new Interval(start, end));
            }
            return intervals;
        }

        /// <summary>
        /// Wybiera metodę przetwarzania na podstawie wyboru użytkownika.
        /// </summary>
        static ICalculate ChooseProcessingMethod(int choice)
        {
            return choice switch
            {
                1 => new TPLCalculate(),
                2 => new ThreadCalculate(),
                _ => throw new InvalidOperationException("Nieprawidłowy wybór metody przetwarzania.")
            };
        }
    }
}
