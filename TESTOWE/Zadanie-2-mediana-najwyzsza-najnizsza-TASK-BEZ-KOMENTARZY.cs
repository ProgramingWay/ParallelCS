using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CityTemperatureData
{
    public string CityName { get; set; }
    public double[] Temperatures { get; set; }

    public CityTemperatureData(string cityName, double[] temperatures)
    {
        CityName = cityName;
        Temperatures = temperatures;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        // Tworzymy listę przechowującą dane temperatur dla miast
        List<CityTemperatureData> cityTemperatureData = new List<CityTemperatureData>();

        // Generujemy 99 miast o nazwach: Miasto1, Miasto2, ..., Miasto99
        string[] cities = Enumerable.Range(1, 99).Select(i => $"Miasto{i}").ToArray();

        // Dla każdego miasta generujemy 365 temperatur i dodajemy je do listy
        foreach (var city in cities)
        {
            double[] temperatures = GenerateTemperatureData(365);  // Generowanie danych temperatur
            cityTemperatureData.Add(new CityTemperatureData(city, temperatures));  // Dodanie danych do listy
        }

        // Lista do przechowywania zadań równoległych (Task)
        List<Task> tasks = new List<Task>();

        // Zmienna do przechowywania globalnych minimalnych i maksymalnych temperatur
        double globalMin = double.MaxValue;
        double globalMax = double.MinValue;

        // Lista przechowująca mediany temperatur dla każdego miasta
        List<(string city, double median)> cityMedians = new List<(string, double)>();

        // Dzielimy zadania na równoległe wątki, każdy oblicza medianę dla jednego miasta
        foreach (var cityData in cityTemperatureData)
        {
            // Tworzymy zadanie równoległe (Task)
            var task = Task.Run(() =>
            {
                // Obliczenie mediany temperatury dla miasta
                double median = GetMedian(cityData.Temperatures);

                // Synchronizujemy dostęp do wspólnych zasobów (listy i zmiennych globalnych)
                lock (cityTemperatureData)  // Lock zapewnia, że tylko jeden wątek na raz modyfikuje dane
                {
                    // Dodanie wyniku mediany do listy cityMedians
                    cityMedians.Add((cityData.CityName, median));

                    // Wyznaczanie minimalnej i maksymalnej temperatury w danym mieście
                    double cityMin = cityData.Temperatures.Min();
                    double cityMax = cityData.Temperatures.Max();

                    // Synchronizowane wyznaczanie globalnych minimalnych i maksymalnych temperatur
                    if (cityMin < globalMin)
                        globalMin = cityMin;  // Aktualizacja globalnej minimalnej temperatury

                    if (cityMax > globalMax)
                        globalMax = cityMax;  // Aktualizacja globalnej maksymalnej temperatury
                }
            });

            // Dodanie zadania do listy zadań
            tasks.Add(task);
        }

        // Czekamy na zakończenie wszystkich zadań równoległych
        await Task.WhenAll(tasks);

        // Po zakończeniu wszystkich zadań, wyświetlamy wyniki dla każdego miasta
        foreach (var cityMedian in cityMedians)
        {
            Console.WriteLine($"{cityMedian.city}: Mediana temperatur = {cityMedian.median}");
        }

        // Wyświetlamy globalne minimalne i maksymalne temperatury
        Console.WriteLine($"Globalna minimalna temperatura: {globalMin}");
        Console.WriteLine($"Globalna maksymalna temperatura: {globalMax}");
    }

    // Funkcja do generowania losowych danych temperatur (zakres: 10.0 - 30.0)
    static double[] GenerateTemperatureData(int days)
    {
        // Używamy losowego generatora, który jest inicjowany różnymi wartościami, aby uniknąć powtarzalnych wyników w równoległych wątkach
        Random random = new Random(Guid.NewGuid().GetHashCode());
        double[] temperatures = new double[days];

        // Generowanie 365 temperatur dla każdego miasta
        for (int i = 0; i < days; i++)
        {
            temperatures[i] = Math.Round(random.NextDouble() * (30.0 - 10.0) + 10.0, 1);  // Losowa temperatura w zakresie 10.0 - 30.0
        }

        return temperatures;  // Zwracamy tablicę temperatur
    }

    // Funkcja do obliczania mediany z tablicy temperatur
    static double GetMedian(double[] temperatures)
    {
        // Sortowanie temperatur
        var sortedTemperatures = temperatures.OrderBy(t => t).ToArray();
        int count = sortedTemperatures.Length;

        // Obliczanie mediany na podstawie liczby elementów w tablicy
        if (count % 2 == 0)
        {
            // Dla liczby parzystej, mediana to średnia dwóch środkowych elementów
            return (sortedTemperatures[count / 2 - 1] + sortedTemperatures[count / 2]) / 2.0;
        }
        else
        {
            // Dla liczby nieparzystej, mediana to środkowy element
            return sortedTemperatures[count / 2];
        }
    }
}
