using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Klasa przechowująca dane o temperaturach w danym mieście
public class CityTemperatureData
{
    public string CityName { get; set; } // Nazwa miasta
    public double[] Temperatures { get; set; } // Tablica temperatur dla danego miasta

    // Konstruktor klasy, inicjalizuje nazwę miasta i dane o temperaturach
    public CityTemperatureData(string cityName, double[] temperatures)
    {
        CityName = cityName;
        Temperatures = temperatures;
    }
}

class Program
{
    // Główna metoda aplikacji, używająca asynchronicznego przetwarzania
    static async Task Main(string[] args)
    {
        // Lista obiektów przechowujących dane o temperaturach dla miast
        List<CityTemperatureData> cityTemperatureData = new List<CityTemperatureData>();

        // Tablica z nazwami 99 miast (Miasto1, Miasto2, ..., Miasto99)
        string[] cities = Enumerable.Range(1, 99).Select(i => $"Miasto{i}").ToArray();

        // Generowanie danych temperatur dla każdego miasta
        foreach (var city in cities)
        {
            // Generowanie 365 losowych temperatur dla każdego miasta
            double[] temperatures = GenerateTemperatureData(365);
            // Dodanie obiektu CityTemperatureData do listy
            cityTemperatureData.Add(new CityTemperatureData(city, temperatures));
        }

        // Lista zadań (tasków) do równoległego przetwarzania danych
        List<Task> tasks = new List<Task>();

        // Zmienna przechowująca minimalną temperaturę w całym zestawie danych
        double globalMin = double.MaxValue;
        // Zmienna przechowująca maksymalną temperaturę w całym zestawie danych
        double globalMax = double.MinValue;

        // Lista do przechowywania wyników median dla każdego miasta
        List<(string city, double median)> cityMedians = new List<(string, double)>();

        // Zmienna do monitorowania postępu (ilości przetworzonych miast)
        int processedCities = 0;

        // Przetwarzanie danych w równoległych wątkach (Task)
        foreach (var cityData in cityTemperatureData)
        {
            // Tworzenie zadania równoległego
            var task = Task.Run(() =>
            {
                // Obliczanie mediany temperatury dla danego miasta
                double median = GetMedian(cityData.Temperatures);

                // Blokada synchronizująca dostęp do zmiennych współdzielonych
                lock (cityTemperatureData)
                {
                    // Dodanie wyniku (miasto, mediana) do listy cityMedians
                    cityMedians.Add((cityData.CityName, median));

                    // Obliczanie minimalnej i maksymalnej temperatury w danym mieście
                    double cityMin = cityData.Temperatures.Min();
                    double cityMax = cityData.Temperatures.Max();

                    // Sprawdzanie, czy aktualne wartości minimalne i maksymalne są najniższe/najwyższe w globalnym zbiorze
                    if (cityMin < globalMin)
                        globalMin = cityMin;

                    if (cityMax > globalMax)
                        globalMax = cityMax;
                }
            });

            // Dodanie zadania do listy tasks
            tasks.Add(task);

            // Zwiększanie licznika przetworzonych miast
            processedCities++;
            // Co 20 miast wyświetlanie komunikatu o postępie
            if (processedCities % 20 == 0)
            {
                Console.WriteLine($"Przetworzono {processedCities} miast, proszę czekać...");
            }
        }

        // Czekanie na zakończenie wszystkich zadań
        while (tasks.Any(t => !t.IsCompleted))
        {
            // Opóźnienie 1 sekundy, by sprawdzić status zadań
            await Task.Delay(1000);
        }

        // Oczekiwanie na zakończenie wszystkich zadań przed dalszym przetwarzaniem
        await Task.WhenAll(tasks);

        // Wyświetlanie wyników dla każdego miasta
        foreach (var cityMedian in cityMedians)
        {
            Console.WriteLine($"{cityMedian.city}: Mediana temperatur = {cityMedian.median}");
        }

        // Wyświetlanie globalnych wartości minimalnej i maksymalnej temperatury
        Console.WriteLine($"Globalna minimalna temperatura: {globalMin}");
        Console.WriteLine($"Globalna maksymalna temperatura: {globalMax}");
    }

    // Funkcja do generowania losowych danych temperatur dla określonej liczby dni
    static double[] GenerateTemperatureData(int days)
    {
        // Tworzenie instancji generatora liczb losowych
        Random random = new Random(Guid.NewGuid().GetHashCode());
        // Tablica przechowująca dane o temperaturach
        double[] temperatures = new double[days];

        // Generowanie losowych temperatur dla każdego dnia
        for (int i = 0; i < days; i++)
        {
            temperatures[i] = Math.Round(random.NextDouble() * (30.0 - 10.0) + 10.0, 1);
        }

        return temperatures;
    }

    // Funkcja do obliczania mediany z tablicy temperatur
    static double GetMedian(double[] temperatures)
    {
        // Posortowanie temperatur rosnąco
        var sortedTemperatures = temperatures.OrderBy(t => t).ToArray();
        int count = sortedTemperatures.Length;

        // Sprawdzanie, czy liczba dni (temperatur) jest parzysta czy nieparzysta
        if (count % 2 == 0)
        {
            // Jeśli parzysta, mediana to średnia dwóch środkowych wartości
            return (sortedTemperatures[count / 2 - 1] + sortedTemperatures[count / 2]) / 2.0;
        }
        else
        {
            // Jeśli nieparzysta, mediana to środkowa wartość
            return sortedTemperatures[count / 2];
        }
    }
}
